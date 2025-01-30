using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> Register([FromBody] AuthUserDTO userRequest){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _authService.Register(userRequest);        
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] AuthUserDTO userRequest){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var token = await _authService.Login(userRequest);

        if(token != null){
        Response.Cookies.Append("JwtCookie",token); 
        return Ok();
        }

        return StatusCode(500,"Authentication error");
    }

}