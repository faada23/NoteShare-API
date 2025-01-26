using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    public IUnitOfWork UnitOfWork {get;}

    public AuthService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }
    public Task Login(LoginUserRequest userRequest)
    {
        throw new NotImplementedException();
    }

    public async Task Register(RegisterUserRequest userRequest)
    {
        var user = userRequest.ToUser();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user,user.PasswordHash);
        await UnitOfWork.UserRepository.Insert(user);
        await UnitOfWork.SaveAsync();
    }
}