using API.Core.Models;

public class UnitOfWork : IUnitOfWork
{   
    public DatabaseContext DbContext {get; private set;}
    public IRepository<Role> RoleRepository {get; private set;}

    public IRepository<User> UserRepository {get; private set;}

    public IRepository<Note> NoteRepository {get; private set;}

    public UnitOfWork(DatabaseContext dbContext){
        DbContext = dbContext;
        UserRepository = new Repository<User>(DbContext);
        RoleRepository = new Repository<Role>(DbContext);
        NoteRepository = new Repository<Note>(DbContext);
    }
    public async Task SaveAsync()
    {
        await DbContext.SaveChangesAsync();
    }
}