namespace API.Application.DTOs;

public record CreateNoteRequest
(
    string Title,
    string Content,
    bool IsPublic
);