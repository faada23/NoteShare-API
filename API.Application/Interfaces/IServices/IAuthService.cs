using API.Application.DTOs;

public interface IAuthService 
{   
    public IUnitOfWork UnitOfWork{get;}
    public Task Register (RegisterRequest userRequest);
    public Task<string?> Login (LoginRequest userRequest);
}