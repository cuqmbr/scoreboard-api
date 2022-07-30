using DatabaseModels.Requests;
using DatabaseModels.Responses;
using Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthenticationController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    // POST: /authentication/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Register(request);
        
        if (!success)
        {
            return BadRequest(new AuthenticationResponse {IsError = true, ErrorMessage = content} );
        }

        return await Login(request);
    }
    
    // POST: /authentication/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Login(request);
        
        if (!success)
        {
            return BadRequest(new AuthenticationResponse {IsError = true, ErrorMessage = "Username or password is incorrect."});
        }

        return Ok(new AuthenticationResponse { Token = content } );
    }
}