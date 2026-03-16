using MTGDraft.Enums;

namespace MTGDraft.Features.Draft.DTOs;

public record class CardDTO(
    int ID,
    string Name,
    string Rarity,
    string SetCode,
    int CardNumber,
    int SetId,
    CardTreatment Treatment,
    FoilType FoilType
);
