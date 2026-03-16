using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.Features.Decks.DTOs;

namespace MTGDraft.Features.Decks.Routes;

public static class DeckRoutes
{
    public static void MapDeckRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/Users/{userId}/Players/{playerId}/Decks");

        // get a players decks
        group.MapGet("/", async (int playerId, DraftContext context) =>
        {
            var player = await context.Players.FindAsync(playerId);
            if (player == null)
            {
                return Results.NotFound();
            }

            var decks = await context.Decks
                .Where(d => d.PlayerId == playerId)
                .Select(d => new DeckSummaryDTO(
                    d.Id, 
                    d.Name
                )).ToListAsync();
            return Results.Ok(decks);
        });

        // delete a players deck
        group.MapDelete("/{deckId}", async (int playerId, int deckId, DraftContext context) =>
        {
            var player = await context.Players.FindAsync(playerId);
            if (player == null)
            {
                return Results.NotFound();
            }

            var deck = await context.Decks
                .Where(d => d.Id == deckId && d.PlayerId == playerId)
                .FirstOrDefaultAsync();

            if (deck == null)
            {
                return Results.NotFound();
            }

            context.Decks.Remove(deck);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        // get deck contents
        group.MapGet("/{deckId}", async (int playerId, int deckId, DraftContext context) =>
        {
            var player = await context.Players.FindAsync(playerId);
            if (player == null)
            {
                return Results.NotFound();
            }

            var deck = await context.Decks
                .Where(d => d.Id == deckId && d.PlayerId == playerId)
                .Include(d => d.DeckCards)
                .ThenInclude(dc => dc.Card)
                .FirstOrDefaultAsync();

            if (deck == null)
            {
                return Results.NotFound();
            }

            var deckDTO = new DeckDTO(
                deck.Id,
                deck.Name,
                deck.PlayerId,
                deck.DeckCards.Select(dc => new DeckCardDTO(
                    dc.Id,
                    dc.CardId,
                    dc.Card.Name
                )).ToList()
            );

            return Results.Ok(deckDTO);
        });
    }
}