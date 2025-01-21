using System.ComponentModel.DataAnnotations;

public class Note{
    public int Id {get;set;}
    [Required]
    public string Title {get;set;} = null!;
    [Required]
    public string Content {get;set;} = null!;
    public bool IsPublic {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}

    public int UserId {get;set;}
    public User? User {get;set;}
}