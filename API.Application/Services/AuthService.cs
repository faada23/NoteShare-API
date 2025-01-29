using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    public IUnitOfWork UnitOfWork {get;}

    public IJwtProvider JwtProvider {get;}
    public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider){
        UnitOfWork = unitOfWork;
        JwtProvider = jwtProvider; 
    }
    public async Task<string?> Login(AuthUserDTO userRequest)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username,"Roles");
        if(user != null){

            var passwordCheck = new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,userRequest.Password); 
            if(passwordCheck == PasswordVerificationResult.Success){

                var token = JwtProvider.GenerateToken(user);
                return token;
            }    
        }
        return null;
    }

    public async Task Register(AuthUserDTO userRequest)
    {
        var user = userRequest.ToUser();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user,user.PasswordHash);

        var userRole =  await UnitOfWork.RoleRepository.GetByFilter(p => p.Name == "User");
        user.Roles.Add(userRole);

        await UnitOfWork.UserRepository.Insert(user);
        await UnitOfWork.SaveAsync();
    }
}