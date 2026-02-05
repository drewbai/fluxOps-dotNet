using FluxOps.Application;
using FluxOps.Domain;
using Microsoft.Extensions.Logging;

namespace FluxOps.Infrastructure.Model;

/// <summary>
/// Mock model executor that sums features and emits a simple prediction.
/// </summary>
public sealed class MockModelExecutor : IModelExecutor
{
    private readonly ILogger<MockModelExecutor> _logger;

    /// <summary>
    /// Creates a new <see cref="MockModelExecutor"/>.
    /// </summary>
    public MockModelExecutor(ILogger<MockModelExecutor> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ModelOutput> ExecuteAsync(FeatureSet features, CancellationToken cancellationToken = default)
    {
        var sum = features.Features.Sum();
        _logger.LogInformation("Executed mock model. Feature sum={Sum}", sum);
        return Task.FromResult(new ModelOutput(new[] { sum }, "mock"));
    }
}
