using System.Security.Cryptography;
using API.Application.DTOs;
using API.Core.Models;


namespace API.Application.Mapper;
public static class Mapper {

    public static User ToUser(this RegisterUserRequest userRequest)
    {
        return new User{
            Id = new Guid(),
            Username = userRequest.Username,
            PasswordHash = userRequest.Password,
            CreatedAt = DateTime.UtcNow,       
        };
    }

    public static GetUserRequest ToGetUserRequest(this User user){
        return new GetUserRequest(
           user.Id,
           user.Username,
           user.IsBanned,
           user.CreatedAt
        );
    }
}