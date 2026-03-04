using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Player;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;
using MTGDraft.DTOs.Card;
using MTGDraft.Models;

namespace MTGDraft.Routes;

public static class DraftSessionInfoRoutes
{
    public static void MapDraftSessionInfoRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/DraftSession");

        // get all the draft sessions
        group.MapGet("/", async (DraftContext context) =>
        {
            var sessions = await context.DraftSessions
                .Select(draft => new DraftSessionSummaryDTO(
                    draft.Id,
                    draft.SetCode,
                    draft.PlayerCount,
                    draft.CurrentPickIndex,
                    draft.CurrentPackNumber,
                    draft.DraftPlayers.Select(p => new PlayerSessionSummaryDTO(
                        p.Id,
                        p.Name,
                        p.IsBot,
                        p.DraftSessionId,
                        p.DraftSessionSeat,
                        p.HasPickedThisRound
                    )).ToList(),
                    draft.DraftState,
                    draft.CreatedAt
                )).ToListAsync();
            
            return Results.Ok(sessions);
        });

        // get a specific draft session
        group.MapGet("/{id}", async (int id, DraftContext context) =>
        {
            var session = await context.DraftSessions
                .Where(draft => draft.Id == id)
                .Select(draft => new DraftSessionDTO(
                    draft.Id,
                    draft.SetCode,
                    draft.PlayerCount,
                    draft.CurrentPickIndex,
                    draft.CurrentPackNumber,
                    draft.DraftPlayers.Select(p => new PlayerSummaryDTO(
                        p.Id,
                        p.Name,
                        p.IsBot,
                        p.DraftSessionId,
                        p.DraftSessionSeat
                    )).ToList(),
                    draft.DraftState,
                    draft.CreatedAt,
                    draft.Packs.Select(pack => new PackDTO(
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
                    )).ToList()
                )).FirstOrDefaultAsync();

            if (session is null) {
                return Results.NotFound();
            }

            return Results.Ok(session);
        });
    
        // get a specific player view of draft
        group.MapGet("/{sessionId}/Player/{playerId}", async (int sessionId, int playerId, DraftContext context) =>
        {
            // find the player
            var player = await context.Players
                .Where(player => player.Id == playerId && player.DraftSessionId == sessionId)
                .Select(player => new PlayerSessionSummaryDTO(
                    player.Id, 
                    player.Name, 
                    player.IsBot, 
                    player.DraftSessionId, 
                    player.DraftSessionSeat,
                    player.HasPickedThisRound
                )).FirstOrDefaultAsync();
            if (player == null) return Results.NotFound();

            // get session info
            var session = await context.DraftSessions
                .Where(session => session.Id == sessionId)
                .Select(session => new DraftSessionSummaryDTO(
                    Id: session.Id,
                    SetCode: session.SetCode,
                    PlayerCount: session.PlayerCount,
                    CurrentPickIndex: session.CurrentPickIndex,
                    CurrentPackNumber: session.CurrentPackNumber,
                    Players: session.DraftPlayers
                        .Select(player => new PlayerSessionSummaryDTO(
                            Id: player.Id,
                            Name: player.Name,
                            IsBot: player.IsBot,
                            DraftSessionId: player.DraftSessionId,
                            DraftSessionSeat: player.DraftSessionSeat,
                            HasPicked: player.HasPickedThisRound
                        )).ToList(),
                    DraftState: session.DraftState,
                    CreatedAt: session.CreatedAt
                )).FirstOrDefaultAsync();
            if (session == null) return Results.NotFound($"Draft session {sessionId} not found");

            // get user pack
            var currentPack = await context.Packs
                .Where(pack => pack.DraftSessionId == sessionId && pack.CurrentSeat == player.DraftSessionSeat && pack.PackNumber == session.CurrentPackNumber)
                .SelectMany(pack => pack.Cards)
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
                )).ToListAsync();

            // map to DTO
            var sessionViewDTO = new DraftSessionViewDTO(
                Id: session.Id,
                SetCode: session.SetCode,
                PlayerCount: session.PlayerCount,
                CurrentPickIndex: session.CurrentPickIndex,
                CurrentPackNumber: session.CurrentPackNumber,
                Player: player,
                HasPicked: player.HasPicked,
                DraftState: session.DraftState,
                CurrentPack: currentPack,
                CreatedAt: session.CreatedAt
            );

            return Results.Ok(sessionViewDTO);
        });
    }
}