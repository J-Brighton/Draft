namespace MTGDraft.Features.Decks.DTOs;

public record class DeckCardDTO(
    int Id,
    int CardId,
    string Name
);