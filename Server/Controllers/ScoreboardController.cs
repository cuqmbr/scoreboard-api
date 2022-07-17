using DatabaseModels.DataTransferObjets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<ScoreboardRecordDto[]>> Get()
    {
        return await _sbService.GetScoreboard();
    }

    // GET: /scoreboard/cuqmbr
    [HttpGet("{username}", Name = "Get")]
    public async Task<ActionResult<ScoreboardRecordDto>> Get(string username)
    {
        var sbRecordDto = await _sbService.GetUserHighScore(username);

        if (sbRecordDto == null)
        {
            return NotFound();
        }
        
        return sbRecordDto;
    }

    // POST: /scoreboard
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ScoreboardRecordDto sbRecordDto)
    {
        await _sbService.AddUserHighScore(sbRecordDto);
        return CreatedAtAction(nameof(Get), new {sbRecordDto}, sbRecordDto);
    }
    
    // PUT: /scoreboard/id
    [HttpPut("{id}", Name = "Put")]
    public async Task<ActionResult> Put(int id, [FromBody] ScoreboardRecordDto sbRecordDto)
    {
        if (id != sbRecordDto.Id)
        {
            return BadRequest();
        }

        try
        {
            await _sbService.UpdateScoreboardRecord(sbRecordDto);
        }
        catch (DbUpdateConcurrencyException)
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
