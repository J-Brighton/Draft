using System.ComponentModel.DataAnnotations;

namespace MTGDraft.DTOs.Pack;

public record class UpdatePackDTO(
    [Required] int PackNumber,
    [Required] int OriginalSeat
);

