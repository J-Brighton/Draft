namespace MTGDraft.Features.Decks.DTOs;

public record class DeckDTO(
    int Id,
    string Name,
    int PlayerId,
    List<DeckCardDTO> DeckCards
);