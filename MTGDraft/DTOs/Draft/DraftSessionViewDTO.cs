using MTGDraft.DTOs.Player;
using MTGDraft.DTOs.PackCard;

namespace MTGDraft.DTOs.Draft;

public record class DraftSessionViewDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    int CurrentPickIndex,
    int CurrentPackNumber,
    PlayerSessionSummaryDTO Player,
    bool HasPicked,
    string DraftState,
    List<PackCardDTO> CurrentPack,
    DateTime CreatedAt
);