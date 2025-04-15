using API.Application.DTOs;
using API.Core.Models;


namespace API.Application.Mapper;
public static class Mapper {

    public static User ToUser(this RegisterRequest userRequest)
    {   
        return new User(
            userRequest.Username,
            userRequest.Password,
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

    public static PagedResponse<GetNoteResponse> ToPagedResponse(this PagedList<Note> notes){
        var notesResponse = notes.Select(a => a.ToGetNoteResponse());

        return new PagedResponse<GetNoteResponse>(
            notesResponse.ToList(),
            notes.CurrentPage,
            notes.PageSize,
            notes.TotalItems,
            notes.PageSize
        );
    } 

    public static Note ToNote(this CreateNoteRequest noteRequest,Guid userId){
        var now = DateTime.UtcNow;
        return new Note(
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