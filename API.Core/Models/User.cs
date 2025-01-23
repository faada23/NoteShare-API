using System.ComponentModel.DataAnnotations;

namespace API.Core.Models;
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

    public User(string username,string passwordHash,bool isBanned,DateTime createdAt,List<Role> roles,List<Note> notes){
        Username = username;
        PasswordHash = passwordHash;
        IsBanned = isBanned;
        CreatedAt = createdAt;
        Roles = roles;
        Notes = notes; 
    }
}