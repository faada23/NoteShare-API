using System.Collections.Concurrent;
namespace API.Application.DTOs;

public record UpdateNoteRequest(
    Guid Id,
    string Title,
    string Content
);