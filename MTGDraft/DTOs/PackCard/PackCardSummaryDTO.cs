using MTGDraft.DTOs.Card;

namespace MTGDraft.DTOs.PackCard;

public record class PackCardSummaryDTO(
    int Id,
    bool IsPicked,
    int? PickedByPlayerId
);