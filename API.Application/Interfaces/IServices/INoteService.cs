using API.Application.DTOs;
using API.Core.Models;

public interface INoteService
{
    public IUnitOfWork UnitOfWork{get;}
    public Task<IEnumerable<GetNoteResponse>> GetUserNotes(Guid userId);
    public Task<GetNoteResponse> GetUserNote(Guid id, Guid userId);
    public Task CreateUserNote(CreateNoteRequest noteRequest,Guid userId);
    public Task UpdateUserNote(Guid id, Guid userId);
    public Task DeleteUserNote(Guid id, Guid userId);

}