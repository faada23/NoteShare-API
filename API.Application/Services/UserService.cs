using API.Core.Models;
using API.Application.DTOs;
using API.Application.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    public IUnitOfWork UnitOfWork {get;}
    private readonly ILogger<UserService> _logger;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        UnitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetUserResponse>> GetUser(Guid id)
    {   
        _logger.LogInformation("Attempting to get User {UserId}", id);

        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        if(user == null){
            _logger.LogWarning("Get user failed: User {UserId} not found.", id);
            return Result<GetUserResponse>.Failure("User was not found",ErrorType.RecordNotFound);
        }

        _logger.LogInformation("Successfully retrieved User {UserId}", id);
        return Result<GetUserResponse>.Success(user.ToGetUserResponse());
    }

    public async Task<Result<GetUserResponse>> UpdateUsername(Guid id,string newName)
    {   
        _logger.LogInformation("Attempting to update username for User {UserId} to {NewUsername}", id, newName);
        var user = await UnitOfWork.UserRepository.GetByFilter(
            filter: p => p.Id == id,
            includeProperties: "Roles");

        if (user == null)
        {
            _logger.LogWarning("Update username failed: User {UserId} not found.", id);
            return Result<GetUserResponse>.Failure("User was not found", ErrorType.RecordNotFound);
        }

        foreach(var t in user.Roles)
            if(t.Name == "Moderator"){
                _logger.LogWarning("Update username status failed: User {UserId} is moderator. Default moderator can`t change their username", id);   
                return Result<GetUserResponse>.Failure("Moderator cant change their username",ErrorType.Forbidden); 
            }

        if (user.Username == newName) {
             _logger.LogInformation("Username for User {UserId} is already {NewUsername}. No update needed.", id, newName);
             return Result<GetUserResponse>.Success(user.ToGetUserResponse()); // No change needed
        }

        var existingUser = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == newName);
        if( existingUser != null ){
            _logger.LogWarning("Update username failed: Username {NewUsername} already taken by another User", newName);
            return Result<GetUserResponse>.Failure("This username is already exists",ErrorType.AlreadyExists);
        }

        user.Username = newName;

        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){
            _logger.LogInformation("Successfully updated username for User {UserId} to {NewUsername}", id, newName);
            return Result<GetUserResponse>.Success(user.ToGetUserResponse());
        }

        _logger.LogError("Failed to save username change for User {UserId}. Reason: {FailureReason}", id, result.Message);
        return Result<GetUserResponse>.Failure("Error while changing username",ErrorType.DatabaseError);
    }

    public async Task<Result<bool>> UpdatePassword(Guid id,string newPassword)
    {   
        _logger.LogInformation("Attempting to update password for User {UserId}", id);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);

        if(user == null){
            _logger.LogWarning("Update password failed: User {UserId} not found.", id);
            return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
 
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){
            _logger.LogInformation("Successfully updated password for User {UserId}", id);
            return Result<bool>.Success(true);
        }

        _logger.LogError("Failed to save password change for User {UserId}. Reason: {FailureReason}", id, result.Message);
        return Result<bool>.Failure("Error while changing password",ErrorType.DatabaseError);
    }

    public async Task<Result<bool>> DeleteUser(Guid id){

        _logger.LogInformation("Attempting to delete User {UserId}", id);
        var user = await UnitOfWork.UserRepository.GetByFilter(
            filter: p => p.Id == id,
            includeProperties: "Roles");

        if(user == null){
            _logger.LogWarning("Delete user failed: User {UserId} not found.", id);
            return Result<bool>.Failure("User was not found",ErrorType.RecordNotFound);
        }

        foreach(var t in user.Roles)
            if(t.Name == "Moderator"){
                _logger.LogWarning("Delete user status failed: User {UserId} is moderator.", id);   
                return Result<bool>.Failure("Moderator cant be deleted",ErrorType.Forbidden); 
            }

        UnitOfWork.UserRepository.Delete(user);

        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess){
            _logger.LogInformation("Successfully deleted User {UserId}", id);
            return Result<bool>.Success(true);
        }

        _logger.LogError("Failed to save deletion for User {UserId}. Reason: {FailureReason}", id, result.Message);
        return Result<bool>.Failure("Error deleting a user",ErrorType.DatabaseError);
    }
}