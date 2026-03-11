namespace MTGDraft.DTOs.User;

public record UpdateUserDTO(
    string? Email = null,
    string? Username = null,
    string? Role = null
);