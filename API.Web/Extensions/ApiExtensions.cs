using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class AuthExtension
{
    public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, IConfiguration Configuration)
    {   
        var authSettings = Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();   
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            { 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey))

                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => {
                        context.Token = context.Request.Cookies["MyCookie"];

                        return Task.CompletedTask;
                    }    
                };
            });
        
        serviceCollection.AddAuthorization();
           

        return serviceCollection;    
    }
}