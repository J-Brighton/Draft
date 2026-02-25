using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.DTOs.Deck;
using MTGDraft.DTOs.DeckCard;

namespace MTGDraft.Routes;

public static class DeckRoutes
{
    public static void MapDeckRoutes(this WebApplication app)
    {
        var group = app.MapGroup("Players/{playerId}/Decks");

        // create a deck for a player
        group.MapPost("/", async (int playerId, AddDeckDTO addDeckDTO, DraftContext context) =>
        {
            var player = await context.Players.FindAsync(playerId);
            if (player == null)
            {
                return Results.NotFound();
            }

            var deck = new Deck
            {
                Name = addDeckDTO.Name,
                PlayerId = playerId,
                DeckCards = addDeckDTO.Cards.Select(c => new DeckCard
                {
                    CardId = c.CardId,
                    IsSideboard = c.IsSideboard
                }).ToList()
            };

            context.Decks.Add(deck);
            await context.SaveChangesAsync();
            
            // Reload with includes to populate Card navigation
            var createdDeck = await context.Decks
                .Include(d => d.DeckCards)
                .ThenInclude(dc => dc.Card)
                .FirstAsync(d => d.Id == deck.Id);
            
            var deckDTO = new DeckDTO(
                createdDeck.Id,
                createdDeck.Name,
                createdDeck.PlayerId,
                createdDeck.DeckCards.Select(dc => new DeckCardDTO(
                    dc.Id,
                    dc.CardId,
                    dc.Card.Name
                )).ToList()
            );
            
            return Results.Created($"/Players/{playerId}/Decks/{deck.Id}", deckDTO);
        });

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