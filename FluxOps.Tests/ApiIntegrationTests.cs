using System.Net.Http.Json;
using FluxOps.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FluxOps.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { /* default host */ });
    }

    [Fact]
    public async Task Health_Returns_Ok()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/health");
        res.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Pipeline_Run_Inline_Works()
    {
        var client = _factory.CreateClient();
        var spec = new IngestionSpec
        {
            SourceType = SourceType.Inline,
            Payload = new[] { "abc", "defgh" }
        };

        var res = await client.PostAsJsonAsync("/pipeline/run", spec);
        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<PipelineResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Succeeded);
        Assert.True(body.Metrics?.ContainsKey("mean"));
    }

    private sealed class PipelineResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, double>? Metrics { get; set; }
    }
}
