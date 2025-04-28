using System.ComponentModel.DataAnnotations.Schema;

namespace API.Core.Models;
public class Role{
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {get;set;}
    public string Name {get;set;} = null!;
    public ICollection<User> Users {get;set;} = new List<User>();

    public Role(string name){
        if(name == null || name.Equals(string.Empty)){
            throw new Exception("Name can't be null");
        }

        Name = name;
    }
}