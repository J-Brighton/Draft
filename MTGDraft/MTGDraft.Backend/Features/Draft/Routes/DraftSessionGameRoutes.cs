using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Features.Draft.DTOs;
using MTGDraft.Features.Players.DTOs;

namespace MTGDraft.Features.Draft.Routes;

public static class DraftSessionGameRoutes
{
    public static void MapDraftSessionGameRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/DraftSession");

        group.RequireAuthorization();

        // mark a card picked
        group.MapPost("/{id}/Pick", async (int id, PickPackCardDTO pick, DraftPickService service) =>
        {
            var updatedCard = await service.PickCard(id, pick);
            await service.BotPickCard(id);

            return Results.Ok(new PackCardSummaryDTO(
                Id: updatedCard.Id,
                IsPicked: updatedCard.IsPicked,
                PickedByPlayerId: updatedCard.PickedByPlayerId
            ));
        });

        // advance the gamestate
        group.MapPost("/{id}/Advance", async (int id, DraftFlowService service) =>
        {
            var advancedSession = await service.Advance(id);
            
            return Results.Ok(new DraftSessionSummaryDTO(
                Id: advancedSession.Id,
                SetCode: advancedSession.SetCode,
                PlayerCount: advancedSession.PlayerCount,
                CurrentPickIndex: advancedSession.CurrentPickIndex,
                CurrentPackNumber: advancedSession.CurrentPackNumber,
                Players: advancedSession.DraftPlayers
                    .Select(p => new PlayerSessionSummaryDTO(
                        p.Id,
                        p.Name,
                        p.IsBot,
                        p.DraftSessionId,
                        p.DraftSessionSeat,
                        p.HasPickedThisRound
                    )).ToList(),
                DraftState: advancedSession.DraftState,
                CreatedAt: advancedSession.CreatedAt
            ));
        });
    
        // create decks from drafted cards
        group.MapPost("/{id}/CreateDecks", async (int id, DraftDeckService service) =>
        {
            await service.CreateDraftDecks(id);
            return Results.Ok("decks created successfully");
        });
    
        // clear players from draft
        group.MapPost("/{id}/Clear", async (int id, DraftFlowService service) =>
        {
            await service.ClearSessionId(id);
            return Results.Ok("players cleared from session");
        });
    }
}