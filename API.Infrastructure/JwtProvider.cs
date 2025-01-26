
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Core.Models;

public class JwtProvider(IOptions<JwtOptions> options)
{
    public string GenerateToken(User user)
    {   
        var claims = new List<Claim>{
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
        };
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
        }

        var signingCred = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),SecurityAlgorithms.HmacSha256);
        var expireTime = DateTime.UtcNow.Add(options.Value.Expires);

        var JwtToken = new JwtSecurityToken(
            expires : expireTime,
            claims: claims,
            signingCredentials: signingCred
        );
        
        return new JwtSecurityTokenHandler().WriteToken(JwtToken);
    }
}