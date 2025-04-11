namespace API.Core.Models;
public class Role{
    public Guid Id {get;set;}
    public string Name {get;set;} = null!;
    public ICollection<User> Users {get;set;} = new List<User>();

    public Role(Guid id, string name){
        if(id.Equals(Guid.Empty)){
            throw new Exception("Wrong id format");
        }

        if(name == null || name.Equals(string.Empty)){
            throw new Exception("Name can't be null");
        }

        Id =id;
        Name = name;
    }
}