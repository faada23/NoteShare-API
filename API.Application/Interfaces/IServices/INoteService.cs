using API.Application.DTOs;
using API.Core.Models;

public interface INoteService
{
    public IUnitOfWork UnitOfWork{get;}
    public Task<IEnumerable<GetNoteResponse>> GetUserNotes(Guid userId);
    public Task<GetNoteResponse> GetUserNote(Guid id, Guid userId);
    public Task<bool> CreateUserNote(CreateNoteRequest noteRequest,Guid userId);
    public Task<bool> UpdateUserNote(UpdateNoteRequest id, Guid userId);
    public Task<bool> DeleteUserNote(Guid id, Guid userId);
    public Task<bool> NoteVisibility(Guid id, Guid userId);
}