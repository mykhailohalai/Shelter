using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ShelterAPI.Data;
using ShelterAPI.Services;

namespace ShelterAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Render передає порт через змінну середовища PORT
            var port = Environment.GetEnvironmentVariable("PORT") ?? "3001";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // ─── Services ────────────────────────────────────────────────────

            builder.Services.AddDbContext<BunkerDbContext>(opt =>
                opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
                    ?? "Data Source=bunker.db"));

            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IRoundTableService, RoundTableService>();
            builder.Services.AddScoped<ICardDealingService, CardDealingService>();
            builder.Services.AddScoped<IVotingService, VotingService>();

            var allowedOrigins = (Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "http://localhost:5173")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()));

            builder.Services.AddControllers()
                .AddJsonOptions(opt =>
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            builder.Services.AddOpenApi();

            // ─── Build ───────────────────────────────────────────────────────

            var app = builder.Build();

            // ─── Migrate DB on startup ───────────────────────────────────────

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BunkerDbContext>();
                await db.Database.MigrateAsync();
            }

            // ─── Global error handler ────────────────────────────────────────

            app.UseExceptionHandler(errorApp =>
                errorApp.Run(async context =>
                {
                    var feature = context.Features
                        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    var ex = feature?.Error;

                    var (status, message) = ex switch
                    {
                        KeyNotFoundException       => (404, ex.Message),
                        UnauthorizedAccessException => (403, ex.Message),
                        InvalidOperationException  => (400, ex.Message),
                        _                          => (500, "Внутрішня помилка сервера.")
                    };

                    context.Response.StatusCode = status;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message });
                }));

            // ─── Middleware ──────────────────────────────────────────────────

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(opt =>
                {
                    opt.Title = "Бункер API";
                    opt.Theme = ScalarTheme.DeepSpace;
                });
            }

            app.UseCors();
            app.MapControllers();
            app.Run();
        }
    }
}
