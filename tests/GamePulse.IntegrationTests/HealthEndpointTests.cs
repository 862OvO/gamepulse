using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GamePulse.IntegrationTests;

public sealed class HealthEndpointTests
{
    [Fact]
    public async Task Health_ReturnsHealthyStatus()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("healthy", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("GamePulse.Api", content, StringComparison.Ordinal);
    }
}
