using API.Application.DTOs;
using API.Core.Models;

public interface INoteService
{
    public IUnitOfWork UnitOfWork{get;}
    public Task<Result<PagedResponse<GetNotePreviewResponse>>> GetUserPreviewNotes(Guid userId,PaginationParameters? pagParams);
    public Task<Result<GetNoteResponse>> GetUserNote(Guid id, Guid userId);
    public Task<Result<GetNoteResponse>> CreateUserNote(CreateNoteRequest noteRequest,Guid userId);
    public Task<Result<GetNoteResponse>> UpdateUserNote(UpdateNoteRequest id, Guid userId);
    public Task<Result<Guid>> DeleteUserNote(Guid id, Guid userId);
    public Task<Result<bool>> NoteVisibility(Guid id, Guid userId);
}