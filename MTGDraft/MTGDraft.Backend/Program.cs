using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MTGDraft.Data;
using MTGDraft.Routes;
using MTGDraft.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DraftContext>(options => options.UseSqlite("Data Source=Pack.db"));
builder.Services.AddScoped<DraftSessionService>();
builder.Services.AddScoped<DraftEngineService>();
builder.Services.AddScoped<DraftTimerService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddSignalR();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MTGDraft";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

app.UseCors("Angular");

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthRoutes();
app.MapDraftSessionRoutes();
app.MapDraftSessionInfoRoutes();
app.MapDraftSessionGameRoutes();
app.MapPackRoutes();
app.MapPlayerRoutes();
app.MapDeckRoutes();
app.MapHub<DraftHub>("/DraftHub");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DraftContext>();
    await SeedData.SeedAsync(context);
}

app.Run();
