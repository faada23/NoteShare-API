using System.Security.Claims;
using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class NoteController : ControllerBase{

    private INoteService _noteService {get;}
    private readonly ILogger<NoteController> _logger;

    public NoteController(INoteService noteService, ILogger<NoteController> logger)
    {
        _noteService = noteService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<GetNoteResponse>> CreateNote([FromBody] CreateNoteRequest noteRequest)
    {   
        
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) return NotFound("User not found");

        var result = await _noteService.CreateUserNote(noteRequest,userGuid.Value);
        return  result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> DeleteNote(Guid id)
    {
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) return NotFound("User not found");

        _logger.LogInformation(
            "User {UserId} note {id} delete request (IP: {IP}, Agent: {Agent})",
            userGuid,
            id, 
            GetClientIP(),
            Request.Headers.UserAgent
        );

        var result = await _noteService.DeleteUserNote(id,userGuid.Value);
        return result.ToActionResult<Guid>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetNote(Guid id)
    {   
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) return NotFound("User not found");
        
        var result = await _noteService.GetUserNote(id,userGuid.Value);
        return result.ToActionResult<GetNoteResponse>();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<GetNotePreviewResponse>>> GetNotes(
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");

       var pagParams = (page.HasValue && pageSize.HasValue)
        ? new PaginationParameters { Page = page.Value, PageSize = pageSize.Value }
        : null;

        var result = await _noteService.GetUserPreviewNotes(userGuid.Value,pagParams);
        return result.ToActionResult<PagedResponse<GetNotePreviewResponse>>();
    }

    [HttpPatch]
    public async Task<ActionResult<GetNoteResponse>> UpdateNote([FromBody] UpdateNoteRequest noteRequest)
    {
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");
        
        var result = await _noteService.UpdateUserNote(noteRequest,userGuid.Value);
        return result.ToActionResult<GetNoteResponse>();   
    }

    [HttpPatch("{id}/Visibility")]
    public async Task<ActionResult<bool>> PublishNote(Guid id)
    {
        Guid? userGuid = GetCurrentUserId();
        if(userGuid == null) 
            return NotFound("User not found");

        var result = await _noteService.NoteVisibility(id,userGuid.Value);
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

    private string GetClientIP() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

}