using FluxOps.Application;
using FluxOps.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluxOps.Tests;

public class MLOpsPipelineTests
{
    [Fact]
    public async Task Pipeline_Runs_With_Inline_Payload()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Pipeline:EnableArtifactRouting"] = "true"
            })
            .Build();

        services.AddLogging();
        services.AddFluxOps(config);
        var provider = services.BuildServiceProvider();

        var pipeline = provider.GetRequiredService<IMLOpsPipeline>();
        var spec = new IngestionSpec
        {
            SourceType = SourceType.Inline,
            Payload = new[] { "abc", "defgh" }
        };

        var result = await pipeline.RunAsync(spec);

        Assert.True(result.Succeeded);
        Assert.True(result.Metrics.ContainsKey("mean"));
    }
}
