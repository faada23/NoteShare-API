using API.Application.DTOs;
using API.Core.Models;

public interface ISharedService{

    public IUnitOfWork UnitOfWork{get;}
    public Task<IEnumerable<GetNoteResponse>> GetSharedNotes();
    public Task<GetNoteResponse?> GetSharedNote(Guid id);
}