using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using API.Core.Models;
using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.UserSecrets;

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

        var userGuid = GetCurrentUserId();

        var userRequest = await _userService.GetUser(userGuid);
        return Ok(userRequest);
    }

    [HttpPut("Password")]
    public async Task<ActionResult> UpdatePassword([FromBody] PasswordUpdateRequest updateRequest){

        var userGuid = GetCurrentUserId();

        await _userService.UpdatePassword(userGuid,updateRequest.newPassword);

        return Logout();
    }

    [HttpPut("Username")]
    public async Task<ActionResult> UpdateUsername([FromBody] UsernameUpdateRequest updateRequest){

        var userGuid = GetCurrentUserId();

        await _userService.UpdateUsername(userGuid,updateRequest.newUsername);

        return Logout();
    }

    [HttpPost("Logout")]
    public ActionResult Logout(){
        Response.Cookies.Delete("JwtCookie");
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteCurrentUser(){

        var userGuid = GetCurrentUserId();

        await _userService.DeleteUser(userGuid);
        return Logout();
    
    }

    //helper method
    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue("Id");
        if (!Guid.TryParse(userId, out var userGuid))
            throw new InvalidDataException();
        return userGuid;
    }
}
