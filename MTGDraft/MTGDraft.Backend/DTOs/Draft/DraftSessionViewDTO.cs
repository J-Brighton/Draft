using MTGDraft.DTOs.Player;
using MTGDraft.DTOs.PackCard;
using MTGDraft.Enums;

namespace MTGDraft.DTOs.Draft;

public record class DraftSessionViewDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    int CurrentPickIndex,
    int CurrentPackNumber,
    PlayerSessionSummaryDTO Player,
    bool HasPicked,
    DraftState DraftState,
    List<PackCardDTO> CurrentPack,
    List<PackCardDTO> DraftedCards,
    DateTime CreatedAt
);