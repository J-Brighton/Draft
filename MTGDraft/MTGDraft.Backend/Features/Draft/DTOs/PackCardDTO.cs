using MTGDraft.Enums;

namespace MTGDraft.Features.Draft.DTOs;

public record class PackCardDTO(
    int Id,
    bool IsPicked,
    int? PickedByPlayerId,
    FoilType FoilType,
    CardDTO Card
);