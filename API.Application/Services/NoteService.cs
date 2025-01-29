using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;

public class NoteService : INoteService
{
    public IUnitOfWork UnitOfWork {get;}

    public NoteService(IUnitOfWork unitOfWork){

        UnitOfWork = unitOfWork;
    }

    public async Task CreateUserNote(CreateNoteRequest noteRequest, Guid userId)
    {
        var note = noteRequest.ToNote(userId);
        await UnitOfWork.NoteRepository.Insert(note);
        await UnitOfWork.SaveAsync();
    }

    public Task DeleteUserNote(Guid id, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<GetNoteResponse> GetUserNote(Guid id, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GetNoteResponse>> GetUserNotes(Guid userId)
    {
        var notes = await UnitOfWork.NoteRepository.GetAll();
        return notes.Where(p=> p.UserId == userId).Select(a => a.ToGetNoteResponse());
    }

    public Task UpdateUserNote(Guid id, Guid userId)
    {
        throw new NotImplementedException();
    }
}