using System.Net.Http.Json;
using GamePulse.Web.Models;

namespace GamePulse.Web.Services;

public sealed class GamePulseApiClient(HttpClient httpClient)
{
    public async Task<DashboardSnapshot> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var summaryTask = httpClient.GetFromJsonAsync<DashboardSummaryDto>(
            "api/dashboard/summary",
            cancellationToken);
        var retentionTask = httpClient.GetFromJsonAsync<RetentionReportDto>(
            "api/analytics/retention",
            cancellationToken);
        var funnelTask = httpClient.GetFromJsonAsync<FunnelReportDto>(
            "api/analytics/funnel?targetScore=512",
            cancellationToken);
        var trendTask = httpClient.GetFromJsonAsync<IReadOnlyList<DailyTrendPointDto>>(
            "api/analytics/trend",
            cancellationToken);

        await Task.WhenAll(summaryTask, retentionTask, funnelTask, trendTask);

        return new DashboardSnapshot(
            await summaryTask ?? new DashboardSummaryDto(0, 0, 0, 0, 0),
            await retentionTask ?? new RetentionReportDto(0, 0, 0),
            await funnelTask ?? new FunnelReportDto(512, 0, []),
            await trendTask ?? []);
    }

    public async Task<ImportResultDto> ImportJsonAsync(
        Stream jsonStream,
        CancellationToken cancellationToken = default)
    {
        using var content = new StreamContent(jsonStream);
        content.Headers.ContentType = new("application/json");

        using var response = await httpClient.PostAsync(
            "api/imports/json",
            content,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ImportResultDto>(cancellationToken)
            ?? new ImportResultDto(0, 0, 0, ["API 未返回导入结果。"]);
    }
}
