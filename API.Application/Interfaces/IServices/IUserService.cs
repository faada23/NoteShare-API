using API.Application.DTOs;
using API.Core.Models;

public interface IUserService {

    public IUnitOfWork UnitOfWork {get;}
    Task<GetUserResponse> GetUser(Guid id);
    Task<bool> UpdateUsername(Guid id, string newUsername);
    Task<bool> UpdatePassword(Guid id, string newPassword);

    Task<bool> DeleteUser(Guid id);
}