using System.Dynamic;
using API.Core.Models;
using API.Application.DTOs;

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

    public async Task CreateUser(User user)
    {
        await UnitOfWork.UserRepository.Insert(user);
    }

    public Task UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }
}