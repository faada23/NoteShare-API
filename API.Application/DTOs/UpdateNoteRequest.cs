using System.Collections.Concurrent;

public record UpdateNoteRequest(
    Guid id,
    string Title,
    string content
);