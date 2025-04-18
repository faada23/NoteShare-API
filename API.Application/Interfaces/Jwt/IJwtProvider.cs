using API.Core.Models;

public interface IJwtProvider
{
    public string GenerateToken(User user);
}