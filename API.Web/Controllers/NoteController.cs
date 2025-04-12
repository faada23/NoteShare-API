using System.Security.Claims;
using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class NoteController : ControllerBase{
    
    private IUnitOfWork _unitOfWork {get;}
    private INoteService _noteService {get;}

    public NoteController(IUnitOfWork unitOfWork, INoteService noteService)
    {
        _unitOfWork = unitOfWork;
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateNote([FromBody] CreateNoteRequest noteRequest)
    {   
        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            await _noteService.CreateUserNote(noteRequest,userGuid.Value);
            return Ok();
        }
        return NotFound("User not found");
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteNote(Guid id)
    {
        Guid? userGuid = GetCurrentUserId();
        if(userGuid != null)
        {
            await _noteService.DeleteUserNote(id,userGuid.Value);
            return Ok();
        }
        return NotFound("User not found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetNoteResponse?>> GetNote(Guid id)
    {   
        Guid? userGuid = GetCurrentUserId();

        Log.Information($"User: {userGuid} invoked method GetNote , note: {id}");

        if(userGuid != null)
        {
            var note = await _noteService.GetUserNote(id,userGuid.Value);

            if(note != null)
                return Ok(note);
            
            return NotFound("Note not found");
        }
        return NotFound("User not found");

    }

    [HttpGet]
    public async Task<ActionResult<List<GetNoteResponse>>> GetNotes()
    {
        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            var notes = await _noteService.GetUserNotes(userGuid.Value);
            return Ok(notes);
        }

        return NotFound("User not found");
    }

    [HttpPut]
    public async Task<ActionResult> UpdateNote([FromBody] UpdateNoteRequest noteRequest)
    {
        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            await _noteService.UpdateUserNote(noteRequest,userGuid.Value);
                return Ok();   
        }

        return NotFound("User not found");
    }

    [HttpPut("{id}/Visibility")]
    public async Task<ActionResult> PublishNote(Guid id)
    {
        Guid? userGuid = GetCurrentUserId();

        if(userGuid != null)
        {
            await _noteService.NoteVisibility(id,userGuid.Value);
            return Ok();
        }
        return NotFound("User not found");
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