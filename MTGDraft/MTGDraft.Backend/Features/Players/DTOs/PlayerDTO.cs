using MTGDraft.Features.Decks.DTOs;

namespace MTGDraft.Features.Players.DTOs;

public record class PlayerDTO(
    int Id,
    string Name,
    bool IsBot,
    int? DraftSessionId,
    int? DraftSessionSeat,
    List<DeckSummaryDTO> Decks
);