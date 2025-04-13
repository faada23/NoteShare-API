using API.Core.Models;

public interface IUnitOfWork : IDisposable
{   
    bool Disposed {get;}
    IRepository<Role> RoleRepository { get; }
    IRepository<User> UserRepository { get; }
    IRepository<Note> NoteRepository { get; }

    Task<Result<int>> SaveAsync();
}