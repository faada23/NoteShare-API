using API.Core.Models;

public class NoteService : INoteService
{
    public IUnitOfWork unitOfWork => throw new NotImplementedException();

    public Task CreateNote()
    {
        throw new NotImplementedException();
    }

    public Task DeleteNote()
    {
        throw new NotImplementedException();
    }

    public Task<Note> GetNote(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Note>> GetNotes()
    {
        throw new NotImplementedException();
    }

    public Task UpdateNote()
    {
        throw new NotImplementedException();
    }
}