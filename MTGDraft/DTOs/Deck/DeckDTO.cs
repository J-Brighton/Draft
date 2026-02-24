using MTGDraft.DTOs.DeckCard;

namespace MTGDraft.DTOs.Deck;

public record class DeckDTO(
    int Id,
    string Name,
    List<DeckCardDTO> DeckCards,
    List<DeckCardDTO> SideboardCards
);