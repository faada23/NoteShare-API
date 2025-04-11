namespace API.Core.Models;
public class User {
    public Guid Id {get;set;}
    public string Username {get;set;} = null!;
    public string PasswordHash {get;set;} = null!;
    public  bool IsBanned {get;set;}
    public DateTime CreatedAt {get;set;}
    
    public ICollection<Role> Roles {get;set;} = new List<Role>();
    public ICollection<Note> Notes {get;set;} = new List<Note>();

    public User(Guid id, string username, string passwordHash, bool isBanned, DateTime createdAt){

        if(id.Equals(Guid.Empty)){
            throw new Exception("Wrong id format");
        }

        if(username == null || username == string.Empty){
            throw new Exception("Username can't be empty");
        }

        if(passwordHash == null || passwordHash == string.Empty){
            throw new Exception("Password can't be empty");
        }

        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        IsBanned = isBanned;
        CreatedAt = createdAt;


    }
}