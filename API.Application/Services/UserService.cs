using System.Dynamic;
using API.Core.Models;
using API.Application.DTOs;
using API.Application.Mapper;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using System.IO.Compression;

public class UserService : IUserService
{

    public IUnitOfWork UnitOfWork {get;}

    public UserService(IUnitOfWork unitOfWork){
        UnitOfWork = unitOfWork;
    }

    public async Task<GetUserResponse> GetUser(Guid id)
    {   
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        return user.ToGetUserResponse();
    }

    public async Task UpdateUsername(Guid id,string newName)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        user.Username = newName;
        await UnitOfWork.UserRepository.Update(user);
        await UnitOfWork.SaveAsync();
    }

    public async Task UpdatePassword(Guid id,string newPassword)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
        await UnitOfWork.UserRepository.Update(user); 
        await UnitOfWork.SaveAsync();
    }

    public async Task DeleteUser(Guid id){
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        await UnitOfWork.UserRepository.Delete(id);
        await UnitOfWork.SaveAsync();
    }
}