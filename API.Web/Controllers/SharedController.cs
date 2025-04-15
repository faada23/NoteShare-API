using API.Application.DTOs;
using API.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class SharedController : ControllerBase{
    private ISharedService _sharedService{get;}

    public SharedController(ISharedService sharedService){
        _sharedService = sharedService;
    }


    [HttpGet("notes")]
    public async Task<ActionResult<PagedResponse<GetNoteResponse>>> GetSharedNotes(
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        var pagParams = (page.HasValue && pageSize.HasValue)
        ? new PaginationParameters { Page = page.Value, PageSize = pageSize.Value }
        : null;

        var result = await _sharedService.GetSharedNotes(pagParams);
        return result.ToActionResult<PagedResponse<GetNoteResponse>>();
    }

    [HttpGet("note/{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetSharedNote(Guid id){
        var result = await _sharedService.GetSharedNote(id);
        return result.ToActionResult<GetNoteResponse>();
    }
}