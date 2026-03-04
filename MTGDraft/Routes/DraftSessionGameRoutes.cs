using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.DTOs.Draft;
using MTGDraft.DTOs.Player;
using MTGDraft.DTOs.Pack;
using MTGDraft.DTOs.PackCard;
using MTGDraft.DTOs.Card;
using System.Security.Cryptography.X509Certificates;

namespace MTGDraft.Routes;

public static class DraftSessionGameRoutes
{
    public static void MapDraftSessionGameRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/DraftSession");

        // mark a card picked
        group.MapPost("/{id}/Pick", async (int id, PickPackCardDTO pick, DraftEngineService service) =>
        {
            try
            {
                var updatedCard = await service.PickCard(id, pick);

                await service.BotPickCard(id);

                var updatedCardDTO = new PackCardSummaryDTO(
                    Id: updatedCard.Id,
                    IsPicked: updatedCard.IsPicked,
                    PickedByPlayerId: updatedCard.PickedByPlayerId
                );

                return Results.Ok(updatedCardDTO);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        // advance the gamestate
        group.MapPost("/{id}/Advance", async (int id, DraftEngineService service) =>
        {
            try
            {
                var advancedSession = await service.Advance(id);

                await service.BotPickCard(id);
                
                var advancedSessionDTO = new DraftSessionSummaryDTO(
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
                );

                return Results.Ok(advancedSessionDTO);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}