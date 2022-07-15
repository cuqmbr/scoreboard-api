using DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScoreboardController : ControllerBase
{
    private readonly ServerDbContext _context;
    
    public ScoreboardController(ServerDbContext context)
    {
        _context = context;
    }
    
    // GET: api/Scoreboard
    [HttpGet]
    public async Task<ActionResult<ScoreboardRecord[]>> Get()
    {
        return await _context.Scoreboard.ToArrayAsync();
    }

    // GET: api/Scoreboard/cuqmbr
    [HttpGet("{username}", Name = "Get")]
    public async Task<ActionResult<ScoreboardRecord>> Get(string username)
    {
        var sbRecord = await _context.Scoreboard.FirstOrDefaultAsync(sbr => sbr.Username == username);

        if (sbRecord == null)
        {
            return NotFound();
        }
        
        return sbRecord;
    }

    // POST: api/Scoreboard
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ScoreboardRecord sbRecord)
    {
        await _context.AddAsync(sbRecord);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new {sbRecord.Username}, sbRecord);
    }

    // PUT: api/Scoreboard/cuqmbr
    [HttpPut("{username}")]
    public async Task<ActionResult> Put(string username, [FromBody] ScoreboardRecord sbRecord)
    {
        if (username != sbRecord.Username)
        {
            return BadRequest();
        }

        _context.Entry(sbRecord).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ScoreboardRecordExists(sbRecord.Username))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    private async Task<bool> ScoreboardRecordExists(string username)
    {
        return await _context.Scoreboard.AnyAsync(sbr => sbr.Username == username);
    }
}
