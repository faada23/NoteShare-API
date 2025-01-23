using System.ComponentModel.DataAnnotations;

public class Role{
    public Guid Id {get;set;}
    [Required]
    public string Name {get;set;} = null!;

    public ICollection<User>? users {get;set;}
}