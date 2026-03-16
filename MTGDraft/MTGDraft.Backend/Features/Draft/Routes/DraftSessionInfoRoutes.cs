using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Models;
using MTGDraft.Features.Draft.DTOs;
using MTGDraft.Features.Players.DTOs;
using System.Reflection.Emit;
using System.Security.Claims;

namespace MTGDraft.Features.Draft.Routes;

public static class DraftSessionInfoRoutes
{
    public static void MapDraftSessionInfoRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/api/DraftSession");

        group.RequireAuthorization();

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
                .Where(d => d.Id == id)
                .Select(d => new DraftSessionDTO(
                    d.Id,
                    d.SetCode,
                    d.PlayerCount,
                    d.CurrentPickIndex,
                    d.CurrentPackNumber,
                    d.DraftPlayers.Select(p => new PlayerSummaryDTO(
                        p.Id,
                        p.Name,
                        p.IsBot
                    )).ToList(),
                    d.DraftState,
                    d.CreatedAt,
                    d.Packs.Select(p => new PackDTO(
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
                    )).ToList()
                )).FirstOrDefaultAsync();

            if (session is null) {
                return Results.NotFound();
            }

            return Results.Ok(session);
        });
    
        // get a specific player view of draft
        group.MapGet("/{sessionId}/self", async (int sessionId, DraftContext context, HttpContext http) =>
        {
            // get auth user id
            if (!AuthHelper.GetUserId(http, out var userId)) return Results.Unauthorized();

            // find the player that belongs to the user
            var player = await context.Players
                .Where(p => p.UserId == userId && p.DraftSessionId == sessionId)
                .Select(p => new PlayerSessionSummaryDTO(
                    p.Id, 
                    p.Name, 
                    p.IsBot, 
                    p.DraftSessionId, 
                    p.DraftSessionSeat,
                    p.HasPickedThisRound
                )).FirstOrDefaultAsync();
            if (player == null) return Results.Forbid();

            // get session info
            var session = await context.DraftSessions
                .Where(s => s.Id == sessionId)
                .Select(s => new DraftSessionSummaryDTO(
                    Id: s.Id,
                    SetCode: s.SetCode,
                    PlayerCount: s.PlayerCount,
                    CurrentPickIndex: s.CurrentPickIndex,
                    CurrentPackNumber: s.CurrentPackNumber,
                    Players: s.DraftPlayers
                        .Select(dp => new PlayerSessionSummaryDTO(
                            Id: dp.Id,
                            Name: dp.Name,
                            IsBot: dp.IsBot,
                            DraftSessionId: dp.DraftSessionId,
                            DraftSessionSeat: dp.DraftSessionSeat,
                            HasPicked: dp.HasPickedThisRound
                        )).ToList(),
                    DraftState: s.DraftState,
                    CreatedAt: s.CreatedAt
                )).FirstOrDefaultAsync();
            if (session == null) return Results.NotFound();

            // get user pack
            var currentPack = await context.Packs
                .Where(pack => pack.DraftSessionId == sessionId && pack.CurrentSeat == player.DraftSessionSeat && pack.PackNumber == session.CurrentPackNumber)
                .SelectMany(pack => pack.Cards)
                .Where(pc => !pc.IsPicked)
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
                )).ToListAsync();

            // get user cards
            var draftedCards = await context.PackCards
                .Where(pc => pc.PickedByPlayerId == player.Id)
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
                DraftedCards: draftedCards,
                CreatedAt: session.CreatedAt
            );

            return Results.Ok(sessionViewDTO);
        });
    }
}