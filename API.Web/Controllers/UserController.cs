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
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork,IUserService userService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    [HttpGet()]
    public async Task<ActionResult<GetUserRequest>> GetCurrentUser(){

        string userId = User.FindFirstValue("Id");
        var userGuid = Guid.Parse(userId);
        var userRequest = await _userService.GetUser(userGuid);
        return Ok(userRequest);
    }

    [HttpPut("Password")]
    public async Task<ActionResult> UpdatePassword(string newPassword){

        string userId = User.FindFirstValue("Id");
        var userGuid = Guid.Parse(userId);

        await _userService.UpdatePassword(userGuid,newPassword);

        return await Logout();
    }

    [HttpPut("Username")]
    public async Task<ActionResult> UpdateUsername(string newName){

        string userId = User.FindFirstValue("Id");
        var userGuid = Guid.Parse(userId);

        await _userService.UpdateUsername(userGuid,newName);

        return await Logout();
    }

    [HttpPost("Logout")]
    public async Task<ActionResult> Logout(){
        Response.Cookies.Delete("JwtCookie");
        return Ok();
    }

    [HttpDelete()]
    public async Task<ActionResult> DeleteCurrentUser(){

        string userId = User.FindFirstValue("Id");
        var userGuid = Guid.Parse(userId);

        await _userService.DeleteUser(userGuid);

        return await Logout();
    }
}
