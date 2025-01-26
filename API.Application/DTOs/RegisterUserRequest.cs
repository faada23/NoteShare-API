using API.Core.Models;

namespace API.Application.DTOs;

public record RegisterUserRequest
(
    string Username,
    string Password,
    ICollection<Role>? Roles  
);