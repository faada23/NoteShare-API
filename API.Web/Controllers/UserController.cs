using Microsoft.AspNetCore.Mvc;
using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class UserController : ControllerBase
{   
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet()]
    public async Task<ActionResult<GetUserResponse>> GetCurrentUser(){

        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            var userRequest = await _userService.GetUser(userGuid.Value);
            return Ok(userRequest);
        }
        return NotFound();
    }

    [HttpPut("Password")]
    public async Task<ActionResult> UpdatePassword([FromBody] PasswordUpdateRequest updateRequest){

        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            await _userService.UpdatePassword(userGuid.Value,updateRequest.newPassword); 
            return Logout();
        }

        return NotFound();
    }

    [HttpPut("Username")]
    public async Task<ActionResult> UpdateUsername([FromBody] UsernameUpdateRequest updateRequest){

        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            await _userService.UpdateUsername(userGuid.Value,updateRequest.newUsername);
            return Logout();
        }

        return NotFound();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteCurrentUser(){

        Guid? userGuid = GetCurrentUserId();
        
        if(userGuid != null)
        {
            await _userService.DeleteUser(userGuid.Value);
            return Logout();
        }
        return NotFound();
    
    }

    [HttpDelete("Logout")]
    public ActionResult Logout(){

        Response.Cookies.Delete("JwtCookie");
        return Ok();
    }


    //helper method
    private Guid? GetCurrentUserId()
    {
        var userId = User.FindFirstValue("Id");
        if (!Guid.TryParse(userId, out var userGuid))
            return null;

        return userGuid;
    }
}
