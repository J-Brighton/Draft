namespace MTGDraft.DTOs.Draft;

public record class DraftSessionSummaryDTO(
    int Id,
    string SetCode,
    DateTime CreatedAt
);

