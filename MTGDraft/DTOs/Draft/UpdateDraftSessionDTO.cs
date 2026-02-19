using MTGDraft.DTOs.Pack;

namespace MTGDraft.DTOs.Draft;

public record class UpdateDraftSessionDTO(
    string SetCode,
    DateTime CreatedAt,
    ICollection<PackDTO> Packs
);

