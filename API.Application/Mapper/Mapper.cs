using API.Application.DTOs;
using API.Core.Models;


namespace API.Application.Mapper;
public static class Mapper {

    public static User ToUser(this RegisterRequest userRequest)
    {   
        return new User(
            Guid.NewGuid(),
            userRequest.Username,
            userRequest.Password,
            false,
            DateTime.UtcNow
            );
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
        var now = DateTime.UtcNow;
        return new Note(
            Guid.NewGuid(),
            noteRequest.Title,
            noteRequest.Content,
            noteRequest.isPublic,
            now,
            now,
            userId
        );
    }

    public static void Update(this Note note, UpdateNoteRequest noteRequest){
        note.Title = noteRequest.Title;
        note.Content = noteRequest.content;
        note.UpdatedAt = DateTime.UtcNow;
    }
}