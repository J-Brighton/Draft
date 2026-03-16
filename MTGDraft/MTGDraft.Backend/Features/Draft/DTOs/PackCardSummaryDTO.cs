namespace MTGDraft.Features.Draft.DTOs;

public record class PackCardSummaryDTO(
    int Id,
    bool IsPicked,
    int? PickedByPlayerId
);