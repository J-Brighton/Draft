namespace MTGDraft.Features.Auth.DTOs;

public record UpdateUserDTO(
    string? Email = null,
    string? Username = null,
    string? Role = null
);