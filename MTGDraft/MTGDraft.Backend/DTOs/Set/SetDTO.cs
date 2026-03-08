using MTGDraft.DTOs.Card;

namespace MTGDraft.DTOs.Set;

public record class SetDTO(
    int Id,
    string Code,
    string Name,
    List<CardDTO> Cards
);