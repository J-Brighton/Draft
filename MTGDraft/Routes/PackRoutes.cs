using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Card;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;

namespace MTGDraft.Routes;

public static class PackRoutes
{
    public static void MapPackRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/Packs");

        // get the contents of a pack
        group.MapGet("/{id}", async (int id, DraftContext context) =>
        {
            var pack = await context.Packs
                .Where(pack => pack.Id == id)
                .Select(pack => new PackDTO(
                    pack.Id,
                    pack.PackNumber,
                    pack.OriginalSeat,
                    pack.Cards.Select(packcard => new PackCardDTO(
                        packcard.Id,
                        packcard.IsPicked,
                        packcard.PickedByPlayerId,
                        packcard.IsFoil,
                        new CardDTO(
                            packcard.Card.Id,
                            packcard.Card.Name,
                            packcard.Card.Rarity,
                            packcard.Card.SetCode,  
                            packcard.Card.CardNumber,
                            packcard.Card.SetId
                        )
                    )).ToList()
                )).SingleOrDefaultAsync();

            if (pack is null) {
                return Results.NotFound();
            }

            return Results.Ok(pack);
        });

        // toggle ispicked in a specific card within a pack
        group.MapPost("/{packId}/{id}", async (int packId, int id, UpdatePackCardDTO updateDTO, DraftContext context) =>
        {
            var packCard = await context.PackCards.FindAsync(id);

            if (packCard is null) {
                return Results.NotFound();
            }

            packCard.IsPicked = updateDTO.IsPicked;
            packCard.PickedByPlayerId = updateDTO.PickedByPlayerId;

            await context.SaveChangesAsync();

            return Results.Ok();
        });

        // get a specific card within a pack
        group.MapGet("/{packId}/{id}", async (int packId, int id, DraftContext context) =>
        {
            var packCard = await context.PackCards
                .Where(pc => pc.Id == id && pc.PackId == packId)
                .Select(packcard => new PackCardDTO(
                    packcard.Id,
                    packcard.IsPicked,
                    packcard.PickedByPlayerId,
                    packcard.IsFoil,
                    new CardDTO(
                        packcard.Card.Id,
                        packcard.Card.Name,
                        packcard.Card.Rarity,
                        packcard.Card.SetCode,  
                        packcard.Card.CardNumber,
                        packcard.Card.SetId
                    )
                )).SingleOrDefaultAsync();

            if (packCard is null) {
                return Results.NotFound();
            }

            return Results.Ok(packCard);
        });

    }
}
