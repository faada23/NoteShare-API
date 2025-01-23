using System.ComponentModel.DataAnnotations;

public class User {
    public Guid Id {get;set;}
    [Required]
    public string Username {get;set;} = null!;
    [Required]
    public string PasswordHash {get;set;} = null!;
    public  bool IsBanned {get;set;}
    public DateTime CreatedAt {get;set;}
    
    public ICollection<Role>? Roles {get;set;}
    public ICollection<Note>? Notes {get;set;}
}