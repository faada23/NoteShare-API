using Microsoft.EntityFrameworkCore;

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

        // var users = new[]
        // {
        //     new User
        //     {
        //         Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
        //         Username = "admin",
        //         PasswordHash = "123", // хеш для "admin123"
        //         IsBanned = false,
        //         CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //         Roles = new List<Role>(),
        //         Notes = new List<Note>()
        //     },
        //     new User
        //     {
        //         Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
        //         Username = "user",
        //         PasswordHash = "15124", // хеш для "user123"
        //         IsBanned = false,
        //         CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        //         Roles = new List<Role>(),
        //         Notes = new List<Note>()
        //     }
        // };

        // modelBuilder.Entity<User>().HasData(users);
    }

}