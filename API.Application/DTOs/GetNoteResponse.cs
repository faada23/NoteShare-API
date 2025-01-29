namespace API.Application.DTOs;
public record GetNoteResponse(
    Guid Id,
    string Title,
    string content,
    bool isPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid UserId
);
