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
            (
                adminId,
                "admin",
                "admin123",
                false,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ),
            new User
            (
                userId,
                "user",
                "user123",
                false,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ),
            new User
            (
                guestId,
                "guest",
                "guest123",
                true,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            )
        };

        modelBuilder.Entity<Role>(b =>
        {
            b.HasData(
                new Role (moderatorRoleId, "Moderator" ),  
                new Role (userRoleId, "User" )
            );
            
        });
        
        modelBuilder.Entity<User>().HasData(users);
    }

}