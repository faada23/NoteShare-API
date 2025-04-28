namespace API.Application.DTOs;

public record RegisterRequest(
    string Username,
    string Password
);