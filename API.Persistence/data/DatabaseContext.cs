using Microsoft.EntityFrameworkCore;
using API.Core.Models;
using Microsoft.AspNetCore.Identity;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

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

        var moderatorId1 = Guid.Parse("f42b3e7d-9c5a-4b1f-8b3e-7d9c5a4b1f8b");
        var moderatorId2 = Guid.Parse("a1b2c3d4-e5f6-4a1b-2c3d-4e5f6a1b2c3d");

        var moderatorPassword =  "reallyStrongPassword";

        var userRoleId = Guid.Parse("7e8f9a0b-1c2d-4e8f-9a0b-1c2d4e8f9a0b");
        var moderatorRoleId = Guid.Parse("3f4a5b6c-7d8e-4f4a-5b6c-7d8e4f4a5b6c");

        var moderUsers = new[]
        {
            new User(
                moderatorId1,
                "moder1",
                moderatorPassword,
                false,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ),
            new User(
                moderatorId2,
                "moder2",
                moderatorPassword,
                false,
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            )
        };

        modelBuilder.Entity<User>().HasData(moderUsers);

        modelBuilder.Entity<Role>(b =>
        {
            b.HasData(
                new Role(moderatorRoleId, "Moderator"),
                new Role(userRoleId, "User")
            );

            b.HasMany(x => x.Users)
                .WithMany(x => x.Roles)
                .UsingEntity(
                    "RoleUser",
                    l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("RolesId").HasPrincipalKey(nameof(Role.Id)),
                    r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UsersId").HasPrincipalKey(nameof(User.Id)),
                    je =>
                    {
                        je.HasKey("RolesId", "UsersId"); 
                        je.HasData(
                            new { UsersId = moderatorId1, RolesId = moderatorRoleId },
                            new { UsersId = moderatorId2, RolesId = moderatorRoleId }
                        );
                    });

        });

    }
}