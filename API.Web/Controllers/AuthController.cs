using API.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private IUnitOfWork _unitOfWork {get;}

    private IAuthService _authService {get;}

    public AuthController(IUnitOfWork unitOfWork, IAuthService authService){
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest userRequest){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try{
            await _authService.Register(userRequest);        
        }
        catch(Exception ex){
            return StatusCode(500,"Internal server error");
        }

        return Ok();
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginUserRequest userRequest){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try{
            await _authService.Login(userRequest);        
        }
        catch(Exception ex){
            return StatusCode(500,"Internal server error");
        }

        return Ok();
    }
}