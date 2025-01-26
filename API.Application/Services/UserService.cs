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

    public Task<User> GetMe(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUsername()
    {
        throw new NotImplementedException();
    }

    public Task UpdatePassword()
    {
        throw new NotImplementedException();
    }
}