public record GetUserRequest
(
    Guid Id,
    string Username,
    bool IsBanned,
    DateTime CreatedAt 
);