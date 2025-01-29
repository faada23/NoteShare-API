using API.Application.DTOs;
using API.Core.Models;


namespace API.Application.Mapper;
public static class Mapper {

    public static User ToUser(this AuthUserDTO userRequest)
    {
        return new User{
            Id = new Guid(),
            Username = userRequest.Username,
            PasswordHash = userRequest.Password,
            CreatedAt = DateTime.UtcNow,       
        };
    }

    public static GetUserResponse ToGetUserResponse(this User user){
        return new GetUserResponse(
           user.Id,
           user.Username,
           user.IsBanned,
           user.CreatedAt
        );
    }

    public static GetNoteResponse ToGetNoteResponse(this Note note){
        return new GetNoteResponse(
            note.Id,
            note.Title,
            note.Content,
            note.IsPublic,
            note.CreatedAt,
            note.UpdatedAt,
            note.UserId
        );
    }

    public static Note ToNote(this CreateNoteRequest noteRequest,Guid userId){
        return new Note{
            Id = new Guid(),
            Title = noteRequest.Title,
            Content = noteRequest.Content,
            IsPublic = noteRequest.isPublic,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId
        };
    }
}