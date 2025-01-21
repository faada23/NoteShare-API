

public interface IUnitOfWork 
{
    public Repository<Role> RoleRepository {get; }
    public Repository<User> UserRepository {get; }
    public Repository<Note> NoteRepository {get; }

    void Save();
}