using System.Text.Json.Serialization;
using GamePulse.Application.Analytics;
using GamePulse.Application.Imports;
using GamePulse.Domain.Entities;
using GamePulse.Infrastructure;
using GamePulse.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<GameEventImportService>();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("GamePulse") ?? "Data Source=gamepulse.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GamePulseDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.MapGet("/health", () => Results.Ok(new
{
    service = "GamePulse.Api",
    status = "healthy",
    version = "0.1.0"
}));

app.MapPost("/api/imports/json", async (
    IReadOnlyList<GameEvent> events,
    GameEventImportService importService,
    CancellationToken cancellationToken) =>
    Results.Ok(await importService.ImportAsync(events, cancellationToken)));

app.MapGet("/api/dashboard/summary", async (
    AnalyticsService analyticsService,
    CancellationToken cancellationToken) =>
    Results.Ok(await analyticsService.GetSummaryAsync(cancellationToken)));

app.MapGet("/api/analytics/retention", async (
    AnalyticsService analyticsService,
    CancellationToken cancellationToken) =>
    Results.Ok(await analyticsService.GetDayOneRetentionAsync(cancellationToken)));

app.MapGet("/api/analytics/funnel", async (
    int? targetScore,
    AnalyticsService analyticsService,
    CancellationToken cancellationToken) =>
{
    var threshold = targetScore ?? 512;
    return threshold <= 0
        ? Results.BadRequest(new { error = "targetScore 必须大于 0。" })
        : Results.Ok(await analyticsService.GetFunnelAsync(threshold, cancellationToken));
});

app.MapGet("/api/analytics/trend", async (
    AnalyticsService analyticsService,
    CancellationToken cancellationToken) =>
    Results.Ok(await analyticsService.GetDailyTrendAsync(cancellationToken)));

app.Run();

public partial class Program;
