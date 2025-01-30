using API.Application.DTOs;
using API.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class SharedController : ControllerBase{
    private ISharedService _sharedService{get;}

    public SharedController(ISharedService sharedService){
        _sharedService = sharedService;
    }


    [HttpGet]
    public async Task<ActionResult<List<GetNoteResponse>>> GetSharedNotes(){
        var sharedNotes = await _sharedService.GetSharedNotes();
        return Ok(sharedNotes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetSharedNote(Guid id){
        var sharedNote = await _sharedService.GetSharedNote(id);
        if(sharedNote == null) return NotFound();
        else return Ok(sharedNote);
    }
}