using API.Core.Models;

namespace API.Application.DTOs;

public record UserGet(
    Guid Id,
    string UserName,
    bool IsBanned,
    DateTime CreatedAt,
    ICollection<Note>? Notes);