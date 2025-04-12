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
    private IUnitOfWork _unitOfWork {get;}
    private IAuthService _authService {get;}

    public AuthController(IUnitOfWork unitOfWork, IAuthService authService){
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest userRequest)
    {   
        await _authService.Register(userRequest);
        return Ok();

        
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest userRequest){
        
        var token = await _authService.Login(userRequest);

        if(token != null)
        {
            Response.Cookies.Append("JwtCookie",token); 
            return Ok();
        }

        return StatusCode(500,"Wrong authentication data");
        
    }

}