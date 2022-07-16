using System.Data;
using DatabaseModels.Plain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services;

namespace Server.Controllers;

[Route("[controller]")]
[ApiController]
public class ScoreboardController : ControllerBase
{
    private readonly ScoreboardService _sbService;
    
    public ScoreboardController(ScoreboardService sbService)
    {
        _sbService = sbService;
    }
    
    // GET: /scoreboard
    [HttpGet]
    public async Task<ActionResult<ScoreboardRecord[]>> Get()
    {
        return await _sbService.GetScoreboard();
    }

    // GET: /scoreboard/cuqmbr
    [HttpGet("{username}", Name = "Get")]
    public async Task<ActionResult<ScoreboardRecord>> Get(string username)
    {
        var sbRecord = await _sbService.GetUserHighScore(username);

        if (sbRecord == null)
        {
            return NotFound();
        }
        
        return sbRecord;
    }

    // POST: /scoreboard
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ScoreboardRecord sbRecord)
    {
        await _sbService.AddUserHighScore(sbRecord);
        return CreatedAtAction(nameof(Get), new {sbRecord.User}, sbRecord);
    }
    
    // PUT: /scoreboard/id
    [HttpPut("{id}", Name = "Put")]
    public async Task<ActionResult<ScoreboardRecord>> Put(int id, [FromBody] ScoreboardRecord sbRecord)
    {
        if (id != sbRecord.Id)
        {
            return BadRequest();
        }

        try
        {
            await _sbService.UpdateScoreboardRecord(sbRecord);
        }
        catch (DBConcurrencyException)
        {
            if (!await _sbService.ScoreboardRecordExists(id))
            {
                return NotFound();
            }
            
            throw;
        }

        return NoContent();
    }
    
    // DELETE: /scoreboard/id
    [HttpDelete("{id}", Name = "Delete")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!await _sbService.ScoreboardRecordExists(id))
        {
            return NotFound();
        }

        await _sbService.DeleteScoreboardRecord(id);
        return NoContent();
    }
}
