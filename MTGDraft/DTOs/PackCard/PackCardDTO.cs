using MTGDraft.DTOs.Card;

namespace MTGDraft.DTOs.PackCard;

public record class PackCardDTO(
    int Id,
    bool IsPicked,
    bool IsFoil,
    CardDTO Card
);