using API.Application.DTOs;
using API.Core.Models;

public interface ISharedService{

    public IUnitOfWork UnitOfWork{get;}
    public Task<Result<PagedResponse<GetNotePreviewResponse>>> GetSharedPreviewNotes(PaginationParameters? pagParams);
    public Task<Result<GetNoteResponse>> GetSharedNote(Guid id);
}