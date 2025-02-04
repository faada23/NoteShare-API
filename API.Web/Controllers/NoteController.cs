using System.Security.Claims;
using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class NoteController : ControllerBase{
    
    private IUnitOfWork _unitOfWork {get;}
    private INoteService _noteService {get;}

    public NoteController(IUnitOfWork unitOfWork, INoteService noteService){

        _unitOfWork = unitOfWork;
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateNote([FromBody] CreateNoteRequest noteRequest)
    {   
        var userId = GetCurrentUserId();
        await _noteService.CreateUserNote(noteRequest,userId);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteNote(Guid id)
    {
        var userId = GetCurrentUserId();
        await _noteService.DeleteUserNote(id,userId);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetNote(Guid id)
    {
        var userId = GetCurrentUserId();
        var note = await _noteService.GetUserNote(id,userId);
        if(note != null) return Ok(note);
        else return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<List<GetNoteResponse>>> GetNotes()
    {
        var userId = GetCurrentUserId();
        var notes = await _noteService.GetUserNotes(userId);
        Log.Information("User {userId} invoked GetNotes() => {@notes}",userId,notes);
        return Ok(notes);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateNote([FromBody] UpdateNoteRequest noteRequest)
    {
        var userId = GetCurrentUserId();
        await _noteService.UpdateUserNote(noteRequest,userId);
        return Ok();
    }

    [HttpPut("{id}/Visibility")]
    public async Task<ActionResult> PublishNote(Guid id){
        var userId = GetCurrentUserId();
        await _noteService.NoteVisibility(id,userId);
        return Ok();
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