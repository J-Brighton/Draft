using MTGDraft.DTOs.DeckCard;

namespace MTGDraft.DTOs.Deck;

public record class DeckDTO(
    int Id,
    string Name,
    int PlayerId,
    List<DeckCardDTO> DeckCards
);