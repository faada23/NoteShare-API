using API.Application.DTOs;
using API.Core.Models;

public interface IUserService {

    public IUnitOfWork UnitOfWork {get;}
    Task<GetUserResponse> GetUser(Guid id);
    Task UpdateUsername(Guid id, string newUsername);
    Task UpdatePassword(Guid id, string newPassword);
    Task DeleteUser(Guid id);
}