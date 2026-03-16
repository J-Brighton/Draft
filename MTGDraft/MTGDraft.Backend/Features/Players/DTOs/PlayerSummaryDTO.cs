namespace MTGDraft.Features.Players.DTOs;

public record class PlayerSummaryDTO(
    int Id,
    string Name,
    bool IsBot
);