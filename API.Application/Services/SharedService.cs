using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;

public class SharedService : ISharedService
{
    public IUnitOfWork UnitOfWork {get;}

    public SharedService(IUnitOfWork unitOfWork){

        UnitOfWork = unitOfWork;
    }

    public async Task<GetNoteResponse> GetSharedNote(Guid id)
    {
        var note = await UnitOfWork.NoteRepository.GetByFilter(p => p.Id == id);

        if(note.IsPublic == true ) return note.ToGetNoteResponse();
        return null;
    }

    public async Task<IEnumerable<GetNoteResponse>> GetSharedNotes()
    {
        var notes = await UnitOfWork.NoteRepository.GetAll();

        return notes.Where(p => p.IsPublic == true).Select(p => p.ToGetNoteResponse());
    }
}