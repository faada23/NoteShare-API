public class UnitOfWork : IUnitOfWork
{   
    public DatabaseContext DbContext {get; private set;}
    public Repository<Role> RoleRepository {get; private set;}

    public Repository<User> UserRepository {get; private set;}

    public Repository<Note> NoteRepository {get; private set;}

    public UnitOfWork(DatabaseContext dbContext){
        DbContext = dbContext;
        UserRepository = new Repository<User>(DbContext);
        RoleRepository = new Repository<Role>(DbContext);
        NoteRepository = new Repository<Note>(DbContext);
    }
    public void Save()
    {
        DbContext.SaveChanges();
    }
}