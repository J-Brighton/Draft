namespace MTGDraft.Features.Draft.DTOs;

public record class SetDTO(
    int Id,
    string Code,
    string Name,
    List<CardDTO> Cards
);