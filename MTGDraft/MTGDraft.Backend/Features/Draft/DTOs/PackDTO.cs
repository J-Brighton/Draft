namespace MTGDraft.Features.Draft.DTOs;

public record class PackDTO(
    int Id,
    int PackNumber,
    int OriginalSeat,
    List<PackCardDTO> Cards
);

