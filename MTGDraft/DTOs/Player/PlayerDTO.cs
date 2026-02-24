using MTGDraft.DTOs.Deck;

namespace MTGDraft.DTOs.Player;

public record class PlayerDTO(
    int Id,
    string Name,
    List<DeckSummaryDTO> Decks
);