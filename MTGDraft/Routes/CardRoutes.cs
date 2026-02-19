using MTGDraft.DTOs.Card;

namespace MTGDraft.Routes;

public static class CardRoutes
{
    const string GetCardRoute = "GetCard";


    public static void MapCardsRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/cards");
    }
}
