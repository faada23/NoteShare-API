using API.Core.Models;

namespace API.Application.DTOs;

public record UserCreate(
    string UserName,
    bool IsBanned,
    string Password,
    DateTime CreatedAt,
    ICollection<Note>? Notes,
    ICollection<Role>? Roles   
);