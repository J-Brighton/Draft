using System.ComponentModel.DataAnnotations;

namespace MTGDraft.DTOs.PackCard;

public record class UpdatePackCardDTO(
    [Required] bool IsPicked
);