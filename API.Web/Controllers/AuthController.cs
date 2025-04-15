using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private IAuthService _authService {get;}

    public AuthController(IAuthService authService){
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest userRequest)
    {   
        var result = await _authService.Register(userRequest);

        return result.ToActionResult<Guid>();
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest userRequest){
        
        var result = await _authService.Login(userRequest);

        if(result.IsSuccess) Response.Cookies.Append("JwtCookie",result.Value!); 

        return result.ToActionResult<string>();
        
    }

    [HttpDelete("Logout")]
    public ActionResult Logout(){

        Response.Cookies.Delete("JwtCookie");
        return Ok();
    }
}