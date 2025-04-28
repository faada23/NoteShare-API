public record BanUserRequest
(
    Guid Id,
    bool DeletePublicNotes
);