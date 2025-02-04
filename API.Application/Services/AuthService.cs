using System.Diagnostics.Tracing;
using System.Xml.Serialization;
using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;

public class AuthService : IAuthService
{
    public IUnitOfWork UnitOfWork {get;}
    public IJwtProvider JwtProvider {get;}

    public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider){
        UnitOfWork = unitOfWork;
        JwtProvider = jwtProvider;
    }
    public async Task<string?> Login(LoginRequest userRequest)
    {   using (LogContext.PushProperty("LogType", "Custom"))
        {
            
            var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username,"Roles");
            if(user != null){

                
                var passwordCheck = new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,userRequest.Password); 
                if(passwordCheck == PasswordVerificationResult.Success){

                    var token = JwtProvider.GenerateToken(user);
                    Log.Information("login end"); 
                    return token;
                    
                }
            }
        }
        return null;
    }

    public async Task Register(RegisterRequest userRequest)
    {
        var user = userRequest.ToUser();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user,user.PasswordHash);

        var roles =  await UnitOfWork.RoleRepository.GetAll();
        var userRole = roles.FirstOrDefault(p => p.Name == "User");
        var moderatorRole = roles.FirstOrDefault(p => p.Name == "Moderator");

        if(userRequest.ModeratorCode == "12345") user.Roles.Add(moderatorRole);
        user.Roles.Add(userRole);

        await UnitOfWork.UserRepository.Insert(user);
        await UnitOfWork.SaveAsync();
    }
}