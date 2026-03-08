namespace MTGDraft.DTOs.Deck;

public record class AddDeckDTO(
    string Name,
    List<AddDeckCardDTO> Cards
);

public record class AddDeckCardDTO(
    int CardId,
    bool IsSideboard = false
);
