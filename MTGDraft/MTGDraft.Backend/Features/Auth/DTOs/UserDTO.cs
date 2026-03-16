namespace MTGDraft.Features.Auth.DTOs;

public record UserDTO(
    int Id,
    string Email,
    string Username,
    string Role
);