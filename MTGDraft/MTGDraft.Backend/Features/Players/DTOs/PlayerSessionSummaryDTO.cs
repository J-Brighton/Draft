namespace MTGDraft.Features.Players.DTOs;

public record class PlayerSessionSummaryDTO(
    int Id,
    string Name,
    bool IsBot,
    int? DraftSessionId,
    int? DraftSessionSeat,
    bool HasPicked
);