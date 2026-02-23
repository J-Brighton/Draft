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

    }
}
