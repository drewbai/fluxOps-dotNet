using FluxOps.Application;
using FluxOps.Domain;
using Microsoft.Extensions.Logging;

namespace FluxOps.Infrastructure.Artifacts;

/// <summary>
/// Routes artifacts to console logs as a placeholder sink.
/// </summary>
public sealed class ConsoleArtifactRouter : IArtifactRouter
{
    private readonly ILogger<ConsoleArtifactRouter> _logger;

    /// <summary>
    /// Creates a new <see cref="ConsoleArtifactRouter"/>.
    /// </summary>
    public ConsoleArtifactRouter(ILogger<ConsoleArtifactRouter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task RouteAsync(DataBatch batch, FeatureSet features, ModelOutput output, EvaluationResult evaluation, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Routing artifacts: Source={Source}, Records={Records}, Features={FeatureCount}, Predictions={PredCount}, Metrics={MetricCount}",
            batch.Source, batch.Records.Count, features.Features.Count, output.Predictions.Count, evaluation.Metrics.Count);
        return Task.CompletedTask;
    }
}
