using API.Core.Models;

public interface IUserService {

    public IUnitOfWork UnitOfWork{get;}
    Task<User> GetUserById(Guid id);
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(Guid id);
}