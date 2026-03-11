namespace MTGDraft.DTOs.User;

public record UserDTO(
    int Id,
    string Email,
    string Username,
    string Role
);