namespace MTGDraft.DTOs.Draft;

public record class AddDraftSessionDTO(
    string SetCode,
    int PlayerCount
);

