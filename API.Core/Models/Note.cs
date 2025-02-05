using System.ComponentModel.DataAnnotations;

namespace API.Core.Models;
public class Note{
    public Guid Id {get;set;}
    public string Title {get;set;} = null!;
    public string Content {get;set;} = null!;
    public bool IsPublic {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime UpdatedAt {get;set;}

    public Guid UserId {get;set;}
    public User? User {get;set;}


    public Note(Guid id, string title, string content, bool isPublic, DateTime createdAt, DateTime updatedAt, Guid userId ){

        if(id.Equals(Guid.Empty)){
            throw new Exception("Wrong id format");
        }
        
        if(userId.Equals(Guid.Empty)){
            throw new Exception ("Wrong Id format");
        } 

        if(title == null || title == string.Empty){
            throw new Exception("Title can't be empty");
        }

        if(content == null){
            throw new Exception("content can't be null");
        }

        Id = id;
        Title = title;
        Content = content;
        IsPublic = isPublic;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
    }
}