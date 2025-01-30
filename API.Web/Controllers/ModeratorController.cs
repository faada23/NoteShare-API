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

    
}