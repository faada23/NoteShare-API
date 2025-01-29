using API.Application.DTOs;

public interface IAuthService 
{   
    public IUnitOfWork UnitOfWork{get;}
    public Task Register (AuthUserDTO userRequest);
    public Task<string?> Login (AuthUserDTO userRequest);
}