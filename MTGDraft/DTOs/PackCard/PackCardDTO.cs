using MTGDraft.DTOs.Card;

namespace MTGDraft.DTOs.PackCard;

public record class PackCardDTO(
    int Id,
    bool IsPicked,
    int? PickedByPlayerId,
    bool IsFoil,
    CardDTO Card
);