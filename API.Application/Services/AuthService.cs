using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Identity;


public class AuthService : IAuthService
{
    public IUnitOfWork UnitOfWork {get;}
    public IJwtProvider JwtProvider {get;}

    public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider){
        UnitOfWork = unitOfWork;
        JwtProvider = jwtProvider;
    }
    public async Task<Result<string>> Login(LoginRequest userRequest)
    {       
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username,"Roles");

        if(user != null){

            var passwordCheck = new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,userRequest.Password); 
            if(passwordCheck == PasswordVerificationResult.Success){

                var token = JwtProvider.GenerateToken(user);
                return Result<string>.Success(token);
            }
        }

        return Result<string>.Failure("Wrong login or password",ErrorType.RecordNotFound);
    }

    public async Task<Result<Guid>> Register(RegisterRequest userRequest)
    {
        if( await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username) != null ){
            return Result<Guid>.Failure("This Username is already taken",ErrorType.AlreadyExists);
        }

        var user = userRequest.ToUser();
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user,user.PasswordHash);

        var userRole = await UnitOfWork.RoleRepository.GetByFilter(p=> p.Name == "User");
        user.Roles.Add(userRole);
        
        UnitOfWork.UserRepository.Insert(user);
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess) return Result<Guid>.Success(user.Id);
        else return Result<Guid>.Failure("Error while saving in database",ErrorType.DatabaseError);
    }
}