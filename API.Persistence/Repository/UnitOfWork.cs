using API.Core.Models;


public class UnitOfWork : IUnitOfWork, IDisposable
{   
    private DatabaseContext DbContext { get; set; }
    public bool Disposed { get; private set;}

    public IRepository<Role> RoleRepository { get; }
    public IRepository<User> UserRepository { get; }
    public IRepository<Note> NoteRepository { get; }

    public UnitOfWork(DatabaseContext dbContext){
        DbContext = dbContext;
        UserRepository = new Repository<User>(DbContext);
        RoleRepository = new Repository<Role>(DbContext);
        NoteRepository = new Repository<Note>(DbContext);
    }

    public async Task<Result<int>> SaveAsync()
    {   
        try{
            int changes = await DbContext.SaveChangesAsync();
            return Result<int>.Success(changes);
        }
        catch(Exception ex){
            return Result<int>.Failure(ex.Message,ErrorType.DatabaseError);
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed) return;

        if (disposing)
        {
            DbContext?.Dispose();
        }

        Disposed = true;
    }

}