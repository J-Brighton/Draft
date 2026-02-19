namespace MTGDraft.Routes;

public static class PackRoutes
{
    public static void MapPackRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/packs");

    }
}
