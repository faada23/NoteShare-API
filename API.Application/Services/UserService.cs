using System.Dynamic;

public class UserService : IUserService
{

    public IUnitOfWork UnitOfWork {get;}

    public UserService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task<User> GetUserById(Guid id)
    {
        return await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await UnitOfWork.UserRepository.GetAll();
    }

    public Task CreateUser()
    {
        throw new NotImplementedException();
    }

    public Task UpdateUser()
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser()
    {
        throw new NotImplementedException();
    }
}