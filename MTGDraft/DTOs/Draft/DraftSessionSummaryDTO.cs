using MTGDraft.DTOs.Player;
using MTGDraft.Enums;

namespace MTGDraft.DTOs.Draft;

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

