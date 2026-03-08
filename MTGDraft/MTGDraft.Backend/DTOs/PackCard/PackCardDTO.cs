using MTGDraft.DTOs.Card;
using MTGDraft.Enums;

namespace MTGDraft.DTOs.PackCard;

public record class PackCardDTO(
    int Id,
    bool IsPicked,
    int? PickedByPlayerId,
    FoilType FoilType,
    CardDTO Card
);