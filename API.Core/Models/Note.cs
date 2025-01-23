using System.ComponentModel.DataAnnotations;

namespace API.Core.Models;
public class Note{
    public Guid Id {get;set;}
    [Required]
    public string Title {get;set;} = null!;
    [Required]
    public string Content {get;set;} = null!;
    public bool IsPublic {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}

    public Guid UserId {get;set;}
    public User? User {get;set;}
}