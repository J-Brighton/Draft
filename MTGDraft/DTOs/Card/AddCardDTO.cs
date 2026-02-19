namespace MTGDraft.DTOs.Card;

public record class AddCardDTO(
    string Name,
    string Rarity,
    string SetCode,
    int CardNumber
);
