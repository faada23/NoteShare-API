// Data/DatabaseInitializer.cs
using API.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DatabaseInitializer
{
    public async static Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DatabaseContext>();
        const string defaultModeratorName = "Tom Brendy";

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await SeedRoles(context);
            await SeedUsers(context,defaultModeratorName);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {   
            await transaction.RollbackAsync();
            Console.WriteLine("Exception while Initializing Database: "+ex);
        }
    }

    private async static Task SeedRoles(DatabaseContext context)
    {
        if (!await context.Roles.AnyAsync())
        {   
            var moderatorRoleName = "Moderator";
            var userRoleName = "User";

            await context.Roles.AddRangeAsync(
                new Role(moderatorRoleName),
                new Role(userRoleName)
            );
            await context.SaveChangesAsync();
        }
    }

    private async static Task SeedUsers(DatabaseContext context, string defaultModeratorName)
    {   
        
        if (!await context.Users.AnyAsync(u => u.Username == defaultModeratorName))
        {
            var hasher = new PasswordHasher<User>();
            var dateTime = DateTime.UtcNow;

            var MP1 = Environment.GetEnvironmentVariable("NoteShareMP1") 
                    ?? throw new ArgumentNullException("NoteShareMP1", "Moderator password is not set");
            var MP2 = Environment.GetEnvironmentVariable("NoteShareMP2") 
                    ?? throw new ArgumentNullException("NoteShareMP2", "Moderator password is not set");

            var moderatorRole = await context.Roles.FirstAsync(r => r.Name == "Moderator");

            var users = new List<User>
            {
                new User(defaultModeratorName, MP1, dateTime),
                new User("Jeff Lane", MP2, dateTime)
            };

            users.ForEach(u => u.PasswordHash = hasher.HashPassword(u, u.PasswordHash));

            foreach (var user in users)
            {
                user.Roles = new List<Role> { moderatorRole };
            }
            
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}