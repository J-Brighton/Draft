namespace MTGDraft.Features.Draft.DTOs;

public record class AddDraftSessionDTO(
    string SetCode,
    int PlayerCount
);

