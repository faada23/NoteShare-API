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

    public async Task<Result<GetUserResponse>> GetUser(Guid id)
    {   
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        if(user == null) return Result<GetUserResponse>.Failure("User was not found",ErrorType.RecordNotFound);
        return Result<GetUserResponse>.Success(user.ToGetUserResponse());
    }

    public async Task<Result<GetUserResponse>> UpdateUsername(Guid id,string newName)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        var existingUser = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == newName);
        if( existingUser != null ) return Result<GetUserResponse>.Failure("This username is already exists",ErrorType.AlreadyExists);
        
        user.Username = newName;

        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<GetUserResponse>.Success(user.ToGetUserResponse());
        return Result<GetUserResponse>.Failure("Error while changing username",ErrorType.DatabaseError);
    }

    public async Task<Result<bool>> UpdatePassword(Guid id,string newPassword)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if(user == null) return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
 
        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<bool>.Success(true);
        return Result<bool>.Failure("Error while changing password",ErrorType.DatabaseError);
    }

    public async Task<Result<bool>> DeleteUser(Guid id){

        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if(user == null) return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);

        UnitOfWork.UserRepository.Delete(user);

        var result = await UnitOfWork.SaveAsync();
        if(result.IsSuccess) return Result<bool>.Success(true);
        return Result<bool>.Failure("Error while changing password",ErrorType.DatabaseError);
    }
}