using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;


[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Moderator")]
public class ModeratorController : ControllerBase{
    private IModeratorService _moderatorService{get;}

    public ModeratorController(IModeratorService moderatorService){
        _moderatorService = moderatorService;
        }

    //User can't create public notes or share them
    [HttpPatch("users/switchBanStatus")]
    public async Task<ActionResult<bool>> SwitchBanStatus([FromBody] BanUserRequest userRequest){

        var result = await _moderatorService.SwitchBanStatus(userRequest);

        return result.ToActionResult<bool>();
    }

    [HttpDelete("note/{id}")]
    public async Task<ActionResult<Guid>> DeletePublicNote(Guid id){

        var result = await _moderatorService.DeletePublicNote(id);
        
        return result.ToActionResult<Guid>();
    }
    
    private Guid? GetCurrentUserId()
    {
        var userId = User.FindFirstValue("Id");
        if (!Guid.TryParse(userId, out var userGuid))
            return null;

        return userGuid;
    }
}