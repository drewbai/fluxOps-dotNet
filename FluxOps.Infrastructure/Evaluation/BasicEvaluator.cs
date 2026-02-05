using FluxOps.Application;
using FluxOps.Domain;
using Microsoft.Extensions.Logging;

namespace FluxOps.Infrastructure.Evaluation;

/// <summary>
/// Basic evaluator that computes mean of predictions.
/// </summary>
public sealed class BasicEvaluator : IEvaluator
{
    private readonly ILogger<BasicEvaluator> _logger;

    /// <summary>
    /// Creates a new <see cref="BasicEvaluator"/>.
    /// </summary>
    public BasicEvaluator(ILogger<BasicEvaluator> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<EvaluationResult> EvaluateAsync(ModelOutput output, CancellationToken cancellationToken = default)
    {
        var mean = output.Predictions.Count == 0 ? 0.0 : output.Predictions.Average();
        var metrics = new Dictionary<string, double> { { "mean", mean } };
        _logger.LogInformation("Evaluated output. mean={Mean}", mean);
        return Task.FromResult(new EvaluationResult(true, metrics, "basic"));
    }
}
