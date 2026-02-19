namespace MTGDraft.DTOs.Card;

public record class CardDTO(
    int ID,
    string Name,
    string Rarity,
    string SetCode,
    int CardNumber
);
