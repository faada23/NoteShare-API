using System.ComponentModel.DataAnnotations;

public class User {
    public int Id {get;set;}
    public string Username {get;set;}
    public string PasswordHash {get;set;}
    public  bool IsBanned{get;set;}
    public DateTime CreatedAt{get;set;}
    
    public ICollection<Role> Roles {get;set;}
    public ICollection<Note> Notes {get;set;}
}