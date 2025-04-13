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

    public async Task UpdateUsername(Guid id,string newName)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if( await UnitOfWork.UserRepository.GetByFilter(p => p.Username == user.Username) != null )
        
        user.Username = newName;

        UnitOfWork.UserRepository.Update(user);
        await UnitOfWork.SaveAsync();
    }

    public async Task UpdatePassword(Guid id,string newPassword)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);

        UnitOfWork.UserRepository.Update(user); 
        await UnitOfWork.SaveAsync();

    }

    public async Task DeleteUser(Guid id){

        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if(user != null){
            UnitOfWork.UserRepository.Delete(user);
            await UnitOfWork.SaveAsync();
        }

    }
}