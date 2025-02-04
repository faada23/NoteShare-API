using System.Collections.Concurrent;
namespace API.Application.DTOs;

public record UpdateNoteRequest(
    Guid id,
    string Title,
    string content
);