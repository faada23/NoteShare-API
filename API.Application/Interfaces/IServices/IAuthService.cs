using API.Application.DTOs;

public interface IAuthService 
{   
    public IUnitOfWork UnitOfWork{get;}
    public Task Register(RegisterUserRequest userRequest);
    public Task<string?> Login(LoginUserRequest userRequest);
}