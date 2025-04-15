public record BanUserRequest
(
    Guid id,
    bool DeletePublicNotes
);