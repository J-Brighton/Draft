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
        var group = app.MapGroup("api/Users/{userId}/Players");

        group.RequireAuthorization();

        // create a new player for user
        group.MapPost("/", async (int userId, Player player, DraftContext context) =>
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null) return Results.NotFound("User not found");

            player.UserId = userId;
            context.Players.Add(player);
            await context.SaveChangesAsync();

            var playerSummaryDTO = new PlayerSummaryDTO(
                player.Id,
                player.Name,
                player.IsBot
            );

            return Results.Created($"/Users/{userId}/Players/{player.Id}", playerSummaryDTO);
        });

        // delete a player from user
        group.MapDelete("/{playerId}", async (int userId, int playerId, DraftContext context) =>
        {
            var player = await context.Players.FirstOrDefaultAsync(p => p.Id == playerId && p.UserId == userId);
            if (player == null) return Results.NotFound();

            context.Players.Remove(player);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        // get all players for user
        group.MapGet("/", async (int userId, DraftContext context) =>
        {
            var players = await context.Players
                .Where(p => p.UserId == userId)
                .Select(p => new PlayerSummaryDTO(
                    p.Id, 
                    p.Name, 
                    p.IsBot
                )).ToListAsync();
            return Results.Ok(players);
        });

        // get a specific player
        group.MapGet("/{playerId}", async (int userId, int playerId, DraftContext context) =>
        {
            var player = await context.Players.Include(p => p.Decks).FirstOrDefaultAsync(p => p.Id == playerId && p.UserId == userId);
            if (player == null) return Results.NotFound();

            var playerDTO = new PlayerDTO(
                player.Id,
                player.Name,
                player.IsBot,
                player.DraftSessionId,
                player.DraftSessionSeat,
                player.Decks.Select(d => new DeckSummaryDTO(d.Id, d.Name)).ToList()
            );

            return Results.Ok(playerDTO);
        });
    }
}