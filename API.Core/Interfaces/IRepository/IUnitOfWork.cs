using API.Core.Models;
public interface IUnitOfWork 
{
    public IRepository<Role> RoleRepository {get; }
    public IRepository<User> UserRepository {get; }
    public IRepository<Note> NoteRepository {get; }

    Task SaveAsync();
}