using MTGDraft.DTOs.PackCard;

namespace MTGDraft.DTOs.Pack;

public record class PackDTO(
    int Id,
    int PackNumber,
    int OriginalSeat,
    List<PackCardDTO> Cards
);

