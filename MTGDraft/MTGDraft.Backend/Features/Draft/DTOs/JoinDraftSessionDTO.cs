namespace MTGDraft.Features.Draft.DTOs;

public record class JoinDraftSessionDTO(
    string PlayerName,
    bool IsBot
);

