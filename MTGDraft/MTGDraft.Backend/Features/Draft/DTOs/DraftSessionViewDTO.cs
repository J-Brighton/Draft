using MTGDraft.Features.Players.DTOs;
using MTGDraft.Enums;

namespace MTGDraft.Features.Draft.DTOs;

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