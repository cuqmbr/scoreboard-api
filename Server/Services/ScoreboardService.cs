using DatabaseModels.DataTransferObjets;
using DatabaseModels.InitialObjects;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Services;

public class ScoreboardService
{
    private readonly ServerDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ScoreboardService(ServerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET
    
    public async Task<(bool success, string content, ScoreboardRecordDto[])> GetScoreboard()
    {
        var sbRecords = await _dbContext.Scoreboard
            .Include(sbr => sbr.User)
            .ToListAsync();

        sbRecords.Sort((a, b) => b.Score - a.Score);
        sbRecords = sbRecords.DistinctBy(sbr => sbr.User.Id).ToList();

        var dto = new List<ScoreboardRecordDto>(sbRecords.Count);
        foreach (var sbr in sbRecords)
        {
            dto.Add(sbr.ToDto());
        }
        
        return (true, "", dto.ToArray());
    }

    public async Task<(bool success, string content, ScoreboardRecordDto? sbRecordDto)> GetUserHighScore(string username)
    {
        var userScoreboardRecords = await _dbContext.Scoreboard
            .Include(sbr => sbr.User)
            .Where(sbr => sbr.User.Username == username)
            .ToListAsync();

        if (userScoreboardRecords.Count == 0)
        {
            return (false, "Username invalid or scoreboard records are absent", null);
        }

        return (true, "", userScoreboardRecords.MaxBy(sbr => sbr.Score)?.ToDto());
    }

    // POST
    
    public async Task<(bool success, string content)> AddUserHighScore(ScoreboardRecordDto sbRecordDto)
    {
        if (sbRecordDto.User.Id != Int32.Parse(_httpContextAccessor.HttpContext!.User.Claims.First(c => c.Type == "Id").Value))
        {
            return (false, "User id is not yours");
        }

        var sbHighScoreRecordDto = (await GetUserHighScore(sbRecordDto.User.Username)).sbRecordDto;
        
        if (sbHighScoreRecordDto != null &&
            sbRecordDto.Score <= sbHighScoreRecordDto.Score)
        {
            return (false, $"You can not post score lower than {sbHighScoreRecordDto.Score}");
        }

        var sbRecord = new ScoreboardRecord {
            Score = sbRecordDto.Score,
            PostTime = sbRecordDto.PostTime
        };
        var dbUser = await _dbContext.Users.FindAsync(sbRecordDto.User.Id);
        sbRecord.User = dbUser!;
        
        await _dbContext.AddAsync(sbRecord);
        await _dbContext.SaveChangesAsync();
        
        sbRecordDto.Id = _dbContext.ChangeTracker.Entries<ScoreboardRecord>().Last().Entity.Id;

        return (true, "");
    }
    
    // PUT

    public async Task UpdateScoreboardRecord(ScoreboardRecordDto sbRecordDto)
    {
        var sbRecord = new ScoreboardRecord {
            Id = sbRecordDto.Id,
            Score = sbRecordDto.Score,
            PostTime = sbRecordDto.PostTime,
            User = new User {
                Id = sbRecordDto.User.Id,
                Username = sbRecordDto.User.Username
            }
        };
        _dbContext.Entry(sbRecord).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ScoreboardRecordExists(int id)
    {
        return await _dbContext.Scoreboard.AnyAsync(sbr => sbr.Id == id);
    }
    
    // DELETE

    public async Task DeleteScoreboardRecord(int id)
    {
        var sbRecord = await _dbContext.Scoreboard.FindAsync(id);
        _dbContext.Scoreboard.Remove(sbRecord!);
        await _dbContext.SaveChangesAsync();
    }
}