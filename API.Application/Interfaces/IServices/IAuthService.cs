using API.Application.DTOs;

public interface IAuthService 
{   
    public IUnitOfWork UnitOfWork{get;}
    public Task<Result<Guid>> Register (RegisterRequest userRequest);
    public Task<Result<string>> Login (LoginRequest userRequest);
}