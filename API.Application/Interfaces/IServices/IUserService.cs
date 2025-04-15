using API.Application.DTOs;
using API.Core.Models;

public interface IUserService {

    public IUnitOfWork UnitOfWork {get;}
    Task<Result<GetUserResponse>> GetUser(Guid id);
    Task<Result<GetUserResponse>> UpdateUsername(Guid id, string newUsername);
    Task<Result<bool>> UpdatePassword(Guid id, string newPassword);
    Task<Result<bool>> DeleteUser(Guid id);
}