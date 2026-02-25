namespace MTGDraft.DTOs.Player;

public record class PlayerSummaryDTO(
    int Id,
    string Name,
    bool IsBot
);