namespace MTGDraft.DTOs.Card;

public record class UpdateCardDTO(
    string Name,
    string Rarity,
    string SetCode,
    int CardNumber,
    bool isFoil,
    int SetId
);
