namespace API.Application.DTOs;
public record GetNoteResponse(
    Guid Id,
    string Title,
    string Content,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    GetUserResponse User
);
