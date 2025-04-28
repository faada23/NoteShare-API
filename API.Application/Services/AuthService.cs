using API.Application.DTOs;
using API.Application.Mapper;
using API.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    public IUnitOfWork UnitOfWork {get;}
    public IJwtProvider JwtProvider {get;}

     public AuthService(
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger) 
    {
        UnitOfWork = unitOfWork;
        JwtProvider = jwtProvider;
        _logger = logger;
    }
    public async Task<Result<string>> Login(LoginRequest userRequest)
    {   
        _logger.LogInformation("Login attempt for user {Username}", userRequest.Username);
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username,"Roles");

        if(user != null){

            var passwordCheck = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,userRequest.Password); 
            if(passwordCheck == PasswordVerificationResult.Success){
                
                _logger.LogInformation("Password verification successful for user {Username} (ID: {UserId}). Generating token.", user.Username, user.Id);
                
                var token = JwtProvider.GenerateToken(user);
                return Result<string>.Success(token);
            }

            _logger.LogWarning("Login failed: Invalid password for user {Username} (ID: {UserId}).", user.Username, user.Id);
            return Result<string>.Failure("Wrong login or password",ErrorType.InvalidInput);
        }

        _logger.LogWarning("Login failed: User {Username} not found.", userRequest.Username);
        return Result<string>.Failure("Wrong login or password",ErrorType.InvalidInput);
    }

    public async Task<Result<Guid>> Register(RegisterRequest userRequest)
    {   
        _logger.LogInformation("Registration attempt for user {Username}", userRequest.Username);

        if( await UnitOfWork.UserRepository.GetByFilter(p => p.Username == userRequest.Username) != null ){
            _logger.LogWarning("Registration failed: Username {Username} is already taken.", userRequest.Username);
            return Result<Guid>.Failure("This Username is already taken",ErrorType.AlreadyExists);
        }

        var user = userRequest.ToUser();
        user.PasswordHash = _passwordHasher.HashPassword(user,user.PasswordHash);

        var userRole = await UnitOfWork.RoleRepository.GetByFilter(p=> p.Name == "User");
        if(userRole != null)
            user.Roles.Add(userRole);
        else{
            _logger.LogError("Registration failed: Could not save user because default role was not found");
        }

        UnitOfWork.UserRepository.Insert(user);
        var result = await UnitOfWork.SaveAsync();

        if(result.IsSuccess) {
            _logger.LogInformation("User {Username} (ID: {UserId}) registered successfully.", user.Username, user.Id);
            return Result<Guid>.Success(user.Id);
        }

        _logger.LogError("Registration failed: Could not save user {Username} (ID: {UserId}) to the database. Reason: {FailureReason}",
            user.Username, user.Id, result.Message);
        return Result<Guid>.Failure("Error while saving in database",ErrorType.DatabaseError);
    }
}