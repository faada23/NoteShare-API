
public record GetNotePreviewResponse(
    Guid Id,
    string Title,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid UserId,
    string Username
);