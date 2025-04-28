using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private IAuthService _authService {get;}
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService,ILogger<AuthController> logger){
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest userRequest)
    {   
        _logger.LogInformation(
        "Register request for {Username} (Client: {ClientIP}, Agent: {UserAgent})",
        userRequest.Username,
        GetClientIP(),
        Request.Headers.UserAgent
        );

        var result = await _authService.Register(userRequest);

        return result.ToActionResult<Guid>();
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest userRequest){
        
        _logger.LogInformation(
        "Login request for {Username} (Client: {ClientIP}, Agent: {UserAgent})",
        userRequest.Username,
        GetClientIP(),
        Request.Headers.UserAgent
        );

        var result = await _authService.Login(userRequest);

        if(result.IsSuccess) Response.Cookies.Append("JwtCookie",result.Value!); 

        return result.ToActionResult<string>();
        
    }

    [HttpDelete("Logout")]
    public ActionResult Logout(){

        Response.Cookies.Delete("JwtCookie");
        return Ok();
    }

    private string GetClientIP() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}