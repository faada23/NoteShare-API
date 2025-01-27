using Microsoft.EntityFrameworkCore;
using API.Core.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    
    }

    public DbSet<User> Users {get;set;}
    public DbSet<Role> Roles {get;set;}
    public DbSet<Note> Notes {get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        //Only Development?
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var guestId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var userRoleId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        var moderatorRoleId = Guid.Parse("00000000-0000-0000-0000-000000000005");

        var users = new[]
        {
            new User
            {
                Id = adminId,
                Username = "admin",
                PasswordHash = "admin123",
                IsBanned = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<Role>(),
                Notes = new List<Note>()
            },
            new User
            {
                Id = userId,
                Username = "user",
                PasswordHash = "user123",
                IsBanned = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<Role>(),
                Notes = new List<Note>()
            },
            new User
            {
                Id = guestId,
                Username = "guest",
                PasswordHash = "guest123",
                IsBanned = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<Role>(),
                Notes = new List<Note>()
            }
        };

        modelBuilder.Entity<Role>(b =>
        {
            b.HasData(
                new Role { Id = moderatorRoleId, Name = "Moderator" },  
                new Role { Id = userRoleId, Name = "User" }
            );
            
        });
        
        modelBuilder.Entity<User>().HasData(users);
    }

}