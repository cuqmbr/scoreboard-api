using System.Security.Claims;
using DatabaseModels.DataTransferObjets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[Route("api/[controller]")]
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
    [AllowAnonymous]
    public async Task<ActionResult<ScoreboardRecordDto[]>> Get()
    {
        var (success, content, sbRecordsDto) = await _sbService.GetScoreboard();

        if (!success)
        {
            return NotFound();
        }
        
        return sbRecordsDto;
    }

    // GET: /scoreboard/cuqmbr
    [HttpGet("{username}", Name = "Get")]
    [AllowAnonymous]
    public async Task<ActionResult<ScoreboardRecordDto>> Get(string username)
    {
        var (success, content, sbRecordDto) = await _sbService.GetUserHighScore(username);

        if (!success)
        {
            return NotFound(content);
        }
        
        return sbRecordDto!;
    }

    // POST: /scoreboard
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Default, Administrator")]
    public async Task<ActionResult> Post([FromBody] ScoreboardRecordDto sbRecordDto)
    {
        var (success, content) = await _sbService.AddUserHighScore(sbRecordDto);

        if (!success && content.Equals("User id is not yours"))
        {
            return Forbid();
        }

        if (!success && content.Contains("You can not post score lower than"))
        {
            return BadRequest(content);
        }
        
        return CreatedAtAction(nameof(Get), new {sbRecordDto}, sbRecordDto);
    }
    
    // PUT: /scoreboard/id
    [HttpPut("{id}", Name = "Put")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
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
