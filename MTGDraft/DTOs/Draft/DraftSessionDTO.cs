using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.Player;
using MTGDraft.Models;

namespace MTGDraft.DTOs.Draft;

public record class DraftSessionDTO(
    int Id,
    string SetCode,
    int PlayerCount,
    List<PlayerSummaryDTO> Players,
    string DraftState,
    DateTime CreatedAt,
    List<PackDTO> Packs
);

