namespace API.Application.DTOs;

public record GetUserResponse
(
    Guid Id,
    string Username,
    bool IsBanned,
    DateTime CreatedAt 
);