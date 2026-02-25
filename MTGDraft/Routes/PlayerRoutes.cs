using MTGDraft.Data;
using MTGDraft.DTOs.Player;
using MTGDraft.DTOs.Deck;
using MTGDraft.DTOs.DeckCard;
using MTGDraft.Models;
using MTGDraft.Routes;
using Microsoft.EntityFrameworkCore;

namespace MTGDraft.Routes;

public static class PlayerRoutes
{
    public static void MapPlayerRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/Players");

        // create a new player
        group.MapPost("/", async (Player player, DraftContext context) =>
        {
            context.Players.Add(player);
            await context.SaveChangesAsync();
            return Results.Created($"/Players/{player.Id}", player);
        });

        // delete a player
        group.MapDelete("/{id}", async (int id, DraftContext context) =>
        {
            var player = await context.Players.FindAsync(id);
            if (player == null)
            {
                return Results.NotFound();
            }

            context.Players.Remove(player);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        // get all players
        group.MapGet("/", async (DraftContext context) =>
        {
            var players = await context.Players
                .Select(p => new PlayerSummaryDTO(
                    p.Id, 
                    p.Name, 
                    p.IsBot
                )).ToListAsync();
            return Results.Ok(players);
        });

        // get a specific player
        group.MapGet("/{id}", async (int id, DraftContext context) =>
        {
           var player = await context.Players.Include(p => p.Decks).FirstOrDefaultAsync(p => p.Id == id);
           return player != null 
                ? Results.Ok(
                    new PlayerDTO(
                        player.Id, 
                        player.Name, 
                        player.IsBot,
                        player.DraftSessionId,
                        player.Decks.Select(
                            d => new DeckSummaryDTO(d.Id, d.Name)
                        ).ToList())) 
                : Results.NotFound(); 
        });
    }
}