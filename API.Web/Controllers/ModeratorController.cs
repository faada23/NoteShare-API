using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Moderator")]
public class ModeratorController : ControllerBase{
    private IModeratorService _moderatorService{get;}
    private readonly ILogger<ModeratorController> _logger;

    public ModeratorController(IModeratorService moderatorService, ILogger<ModeratorController> logger){
        _logger = logger;
        _moderatorService = moderatorService;
        }

    //User can't create public notes or share them
    [HttpPatch("users/switchBanStatus")]
    public async Task<ActionResult<bool>> SwitchBanStatus([FromBody] BanUserRequest userRequest){
        
        _logger.LogInformation(
            "Ban request | Moderator: {ModeratorId}, Target: {UserId} (IP: {ClientIP})",
            GetCurrentUserId()?.ToString() ?? "unknown",
            userRequest.Id,
            GetClientIP()
        );

        var result = await _moderatorService.SwitchBanStatus(userRequest);

        return result.ToActionResult<bool>();
    }

    [HttpDelete("note/{id}")]
    public async Task<ActionResult<Guid>> DeletePublicNote(Guid id){
        
        _logger.LogInformation(
            "Moderator note delete request | Moderator: {ModeratorId}, Target note: {id} (IP: {ClientIP})",
            GetCurrentUserId()?.ToString() ?? "unknown",
            id,
            GetClientIP()
        );

        var result = await _moderatorService.DeletePublicNote(id);
        
        return result.ToActionResult<Guid>();
    }
    
    private Guid? GetCurrentUserId()
    {
        var userId = User.FindFirstValue("Id");
        if (!Guid.TryParse(userId, out var userGuid)){   
            return null;
        }

        return userGuid;
    }

    private string GetClientIP() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}