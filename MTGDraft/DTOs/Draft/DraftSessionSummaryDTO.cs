using MTGDraft.DTOs.Player;

namespace MTGDraft.DTOs.Draft;

public record class DraftSessionSummaryDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    List<PlayerSummaryDTO> Players,
    string DraftState,
    DateTime CreatedAt
);

