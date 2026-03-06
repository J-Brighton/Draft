using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.Player;
using MTGDraft.Enums;
using MTGDraft.Models;

namespace MTGDraft.DTOs.Draft;

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

