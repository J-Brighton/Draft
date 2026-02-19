using System.ComponentModel.DataAnnotations;

namespace MTGDraft.DTOs.Pack;

public record class AddPackDTO(
    [Required] int DraftSessionID,
    [Required] int PackNumber,
    [Required] int OriginalSeat
);

