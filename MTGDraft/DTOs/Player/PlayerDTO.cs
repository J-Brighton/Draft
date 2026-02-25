using MTGDraft.DTOs.Deck;

namespace MTGDraft.DTOs.Player;

public record class PlayerDTO(
    int Id,
    string Name,
    bool IsBot,
    int? DraftSessionId,
    List<DeckSummaryDTO> Decks
);