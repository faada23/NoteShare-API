using API.Application.DTOs;
using API.Core.Models;

public interface IUserService {

    public IUnitOfWork UnitOfWork {get;}
    Task<User> GetMe(Guid id);
    Task UpdateUsername();
    Task UpdatePassword();
}