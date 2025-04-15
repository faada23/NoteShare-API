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

    [HttpGet("/me")]
    public async Task<ActionResult<GetUserResponse>> GetCurrentUser(){

        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found"); 

        var result = await _userService.GetUser(userGuid.Value);
        return result.ToActionResult<GetUserResponse>();
    }

    [HttpPatch("Password")]
    public async Task<ActionResult<bool>> UpdatePassword([FromBody] PasswordUpdateRequest updateRequest){

        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");

        var result = await _userService.UpdatePassword(userGuid.Value,updateRequest.NewPassword);
        if(result.IsSuccess)
            Logout();
        return result.ToActionResult<bool>();

    }

    [HttpPatch("Username")]
    public async Task<ActionResult<GetUserResponse>> UpdateUsername([FromBody] UsernameUpdateRequest updateRequest){

        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");

        var result = await _userService.UpdateUsername(userGuid.Value,updateRequest.NewUsername);
        if(result.IsSuccess)
            Logout();
        return result.ToActionResult<GetUserResponse>();
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> DeleteCurrentUser(){

        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");

        var result = await _userService.DeleteUser(userGuid.Value);
        if(result.IsSuccess)
            Logout();
        return result.ToActionResult<bool>();
    }

    //helper method
    private Guid? GetCurrentUserId()
    {
        var userId = User.FindFirstValue("Id");
        if (!Guid.TryParse(userId, out var userGuid))
            return null;

        return userGuid;
    }

    private void Logout(){

        Response.Cookies.Delete("JwtCookie");
    }
}
