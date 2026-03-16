using MTGDraft.Features.Players.DTOs;
using MTGDraft.Enums;

namespace MTGDraft.Features.Draft.DTOs;

public record class DraftSessionSummaryDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    int CurrentPickIndex,
    int CurrentPackNumber,
    List<PlayerSessionSummaryDTO> Players,
    DraftState DraftState,
    DateTime CreatedAt
);

