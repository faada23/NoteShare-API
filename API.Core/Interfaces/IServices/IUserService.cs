public interface IUserService {

    public IUnitOfWork UnitOfWork{get;}
    Task<User> GetUserById(Guid id);
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser();
    Task UpdateUser();
    Task DeleteUser();
}