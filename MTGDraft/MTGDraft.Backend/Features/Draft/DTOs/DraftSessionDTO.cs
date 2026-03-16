using MTGDraft.Features.Draft.DTOs;
using MTGDraft.Features.Players.DTOs;
using MTGDraft.Enums;
using MTGDraft.Models;

namespace MTGDraft.Features.Draft.DTOs;

public record class DraftSessionDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    int CurrentPickIndex,
    int CurrentPackNumber,
    List<PlayerSummaryDTO> Players,
    DraftState DraftState,
    DateTime CreatedAt,
    List<PackDTO> Packs
);

