using DatabaseModels.Plain;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Services;

public class ScoreboardService
{
    private readonly ServerDbContext _dbContext;

    public ScoreboardService(ServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET
    
    public async Task<ScoreboardRecord[]> GetScoreboard()
    {
        var sbRecords = await _dbContext.Scoreboard
            .Include(sbr => sbr.User)
            .ToListAsync();

        sbRecords.Sort((a, b) => b.Score - a.Score);

        return sbRecords.DistinctBy(sbr => sbr.User.Id).ToArray();
    }

    public async Task<ScoreboardRecord?> GetUserHighScore(string username)
    {
        var userScoreboardRecords = await _dbContext.Scoreboard
            .Include(sbr => sbr.User)
            .Where(sbr => sbr.User.Username == username)
            .ToListAsync();
        
        return userScoreboardRecords.MaxBy(sbr => sbr.Score);
    }

    // POST
    
    public async Task AddUserHighScore(ScoreboardRecord sbRecord)
    {
        var dbUser = await _dbContext.Users.FindAsync(sbRecord.User.Id);
        sbRecord.User = dbUser;
        
        await _dbContext.AddAsync(sbRecord);
        await _dbContext.SaveChangesAsync();
    }
    
    // PUT

    public async Task UpdateScoreboardRecord(ScoreboardRecord sbRecord)
    {
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