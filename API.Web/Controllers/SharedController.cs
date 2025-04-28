using API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class SharedController : ControllerBase{
    private ISharedService _sharedService{get;}

    public SharedController(ISharedService sharedService){
        _sharedService = sharedService;
    }


    [HttpGet("notes")]
    public async Task<ActionResult<PagedResponse<GetNotePreviewResponse>>> GetSharedPreviewNotes(
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        var pagParams = (page.HasValue && pageSize.HasValue)
        ? new PaginationParameters { Page = page.Value, PageSize = pageSize.Value }
        : null;

        var result = await _sharedService.GetSharedPreviewNotes(pagParams);
        return result.ToActionResult<PagedResponse<GetNotePreviewResponse>>();
    }

    [HttpGet("note/{id}")]
    public async Task<ActionResult<GetNoteResponse>> GetSharedNote(Guid id){
        var result = await _sharedService.GetSharedNote(id);
        return result.ToActionResult<GetNoteResponse>();
    }
}