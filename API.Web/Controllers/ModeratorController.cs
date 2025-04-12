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
    [HttpPut("users/ban")]
    public async Task<ActionResult> BanUser([FromBody] BanUserRequest userRequest){

        await _moderatorService.BanUser(userRequest);
        return Ok();
    }

    [HttpDelete("notes/{id}")]
    public async Task<ActionResult> DeletePublicNote(Guid id){

        await _moderatorService.DeletePublicNote(id);
        return Ok();

    }

    [HttpGet("Logs")]
    public async Task<ActionResult> GetLogs(){
        return Ok();
    }
    
}