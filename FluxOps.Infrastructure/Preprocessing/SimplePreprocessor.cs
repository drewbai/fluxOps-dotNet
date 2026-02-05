using FluxOps.Application;
using FluxOps.Domain;
using Microsoft.Extensions.Logging;

namespace FluxOps.Infrastructure.Preprocessing;

/// <summary>
/// Simple preprocessor that converts records to feature lengths.
/// </summary>
public sealed class SimplePreprocessor : IPreprocessor
{
    private readonly ILogger<SimplePreprocessor> _logger;

    /// <summary>
    /// Creates a new <see cref="SimplePreprocessor"/>.
    /// </summary>
    public SimplePreprocessor(ILogger<SimplePreprocessor> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<FeatureSet> PreprocessAsync(DataBatch batch, CancellationToken cancellationToken = default)
    {
        var features = batch.Records.Select(r => (double)r.Length).ToArray();
        _logger.LogInformation("Preprocessed {Count} records into {Count} features", batch.Records.Count, features.Length);
        return Task.FromResult(new FeatureSet(features));
    }
}
