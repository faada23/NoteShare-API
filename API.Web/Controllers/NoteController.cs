using System.Security.Claims;
using API.Application.DTOs;
using API.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Swashbuckle.AspNetCore.Annotations;

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
    public async Task<ActionResult> DeleteNote()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetNote()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<List<GetNoteResponse>>> GetNotes()
    {
        var userId = GetCurrentUserId();
        var notes = await _noteService.GetUserNotes(userId);
        return Ok(notes);
    }

    [HttpPut]
    public async Task UpdateNote()
    {
        throw new NotImplementedException();
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