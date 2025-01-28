using API.Core.Models;

public interface INoteService
{
    public IUnitOfWork unitOfWork{get;}
    public Task<IEnumerable<Note>> GetNotes();
    public Task<Note> GetNote(Guid id);
    public Task CreateNote();
    public Task UpdateNote();
    public Task DeleteNote();

}