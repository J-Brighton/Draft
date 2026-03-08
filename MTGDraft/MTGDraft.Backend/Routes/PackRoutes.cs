using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Enums;
using MTGDraft.DTOs.Card;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;
using MTGDraft.PackGeneration;

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
                .Where(p => p.Id == id)
                .Select(p => new PackDTO(
                    p.Id,
                    p.PackNumber,
                    p.OriginalSeat,
                    p.Cards.Select(pc => new PackCardDTO(
                        pc.Id,
                        pc.IsPicked,
                        pc.PickedByPlayerId,
                        pc.FoilType,
                        new CardDTO(
                            pc.Card.Id,
                            pc.Card.Name,
                            pc.Card.Rarity,
                            pc.Card.SetCode,  
                            pc.Card.CardNumber,
                            pc.Card.SetId,
                            pc.Card.Treatment,
                            pc.Card.FoilType
                        )
                    )).ToList()
                )).SingleOrDefaultAsync();

            if (pack is null) {
                return Results.NotFound();
            }

            return Results.Ok(pack);
        });

        // get a specific card within a pack
        group.MapGet("/{packId}/{id}", async (int packId, int id, DraftContext context) =>
        {
            var packCard = await context.PackCards
                .Where(pc => pc.Id == id && pc.PackId == packId)
                .Select(pc => new PackCardDTO(
                    pc.Id,
                    pc.IsPicked,
                    pc.PickedByPlayerId,
                    pc.FoilType,
                    new CardDTO(
                        pc.Card.Id,
                        pc.Card.Name,
                        pc.Card.Rarity,
                        pc.Card.SetCode,  
                        pc.Card.CardNumber,
                        pc.Card.SetId,
                        pc.Card.Treatment,
                        pc.Card.FoilType
                    )
                )).SingleOrDefaultAsync();

            if (packCard is null) {
                return Results.NotFound();
            }

            return Results.Ok(packCard);
        });

        // generate a pack for debug
        app.MapGet("/debug/pack", async (DraftContext context) =>
        {
            var set = await context.Sets.Where(s => s.Code == "ECL").Include(s => s.Cards).SingleOrDefaultAsync();
            if (set is null) return Results.NotFound("didnt find ECL");

            var generator = new PackGenerator();
            var packs = generator.GeneratePacks(set, playerCount: 1);
            var pack = packs.First();

            foreach (var pc in pack.Cards)
            {
                pc.Card = set.Cards.Single(c => c.Id == pc.CardId);
            }

            var dto = new
            {
                pack.PackNumber,
                pack.OriginalSeat,
                Cards = pack.Cards.Select(pc => new PackCardDTO(
                    pc.Id,
                    pc.IsPicked,
                    pc.PickedByPlayerId,
                    pc.FoilType,
                    new CardDTO(
                        pc.Card.Id,
                        pc.Card.Name,
                        pc.Card.Rarity,
                        pc.Card.SetCode,
                        pc.Card.CardNumber,
                        pc.Card.SetId,
                        pc.Card.Treatment,
                        pc.Card.FoilType
                    )
                )).ToList()
            };

            return Results.Ok(dto);
        });

    }
}
