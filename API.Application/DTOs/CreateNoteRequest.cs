public record CreateNoteRequest
(
    string Title,
    string Content,
    bool isPublic
);