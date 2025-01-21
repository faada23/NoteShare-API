using System.ComponentModel.DataAnnotations;

public class Role{
    public int Id {get;set;}
    [Required]
    public string Name {get;set;} = null!;

    public ICollection<User>? users {get;set;}
}