using API.Core.Models;
using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.AspNetCore.Identity;

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

    public async Task<bool> UpdateUsername(Guid id,string newName)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if( await UnitOfWork.UserRepository.GetByFilter(p => p.Username == user.Username) != null )
            return false;
        
        user.Username = newName;

        UnitOfWork.UserRepository.Update(user);
        await UnitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> UpdatePassword(Guid id,string newPassword)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);

        UnitOfWork.UserRepository.Update(user); 
        await UnitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> DeleteUser(Guid id){
        var result = UnitOfWork.UserRepository.Delete(id);
        if(result == false){
            return false;
        }
        await UnitOfWork.SaveAsync();
        return true;
    }
}