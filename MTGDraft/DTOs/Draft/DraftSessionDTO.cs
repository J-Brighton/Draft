using MTGDraft.DTOs.Pack;

namespace MTGDraft.DTOs.Draft;

public record class DraftSessionDTO(
    int Id,
    string SetCode,
    DateTime CreatedAt,
    ICollection<PackDTO> Packs
);

