namespace MTGDraft.Features.Decks.DTOs;

public record class AddDeckDTO(
    string Name,
    List<AddDeckCardDTO> Cards
);

public record class AddDeckCardDTO(
    int CardId,
    bool IsSideboard = false
);
