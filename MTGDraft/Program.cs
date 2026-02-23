using Microsoft.EntityFrameworkCore;
using MTGDraft.Data;
using MTGDraft.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.Services.AddDbContext<DraftContext>(options => options.UseSqlite("Data Source=Pack.db"));
builder.Services.AddScoped<DraftSessionService>();

var app = builder.Build();

app.MapCardsRoutes();
app.MapDraftSessionRoutes();
app.MapPackRoutes();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DraftContext>();
    await SeedData.SeedAsync(context);
}

app.Run();
