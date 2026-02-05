using System.Text;
using FluxOps.Application;
using FluxOps.Domain;
using Microsoft.Extensions.Logging;

namespace FluxOps.Infrastructure.Ingestion;

/// <summary>
/// Default ingestion supporting inline payloads and local files.
/// </summary>
public sealed class DefaultDataIngestion : IDataIngestion
{
    private readonly ILogger<DefaultDataIngestion> _logger;

    /// <summary>
    /// Creates a new <see cref="DefaultDataIngestion"/>.
    /// </summary>
    public DefaultDataIngestion(ILogger<DefaultDataIngestion> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DataBatch> IngestAsync(IngestionSpec spec, CancellationToken cancellationToken = default)
    {
        switch (spec.SourceType)
        {
            case SourceType.Inline:
                var payload = spec.Payload ?? Array.Empty<string>();
                _logger.LogInformation("Ingested inline payload with {Count} records", payload.Count);
                return new DataBatch("inline", DateTimeOffset.UtcNow, payload);

            case SourceType.LocalFile:
                if (string.IsNullOrWhiteSpace(spec.PathOrUri))
                {
                    throw new ArgumentException("PathOrUri must be provided for LocalFile ingestion.");
                }
                var path = spec.PathOrUri;
                _logger.LogInformation("Reading local file: {Path}", path);
                var lines = await ReadAllLinesAsync(path!, cancellationToken).ConfigureAwait(false);
                return new DataBatch(path!, DateTimeOffset.UtcNow, lines);

            default:
                // For CloudObject/Event, this is a placeholder; extend with proper handlers.
                _logger.LogWarning("Unsupported SourceType {Type}. Returning empty batch.", spec.SourceType);
                return new DataBatch(spec.PathOrUri ?? spec.SourceType.ToString(), DateTimeOffset.UtcNow, Array.Empty<string>());
        }
    }

    private static async Task<string[]> ReadAllLinesAsync(string path, CancellationToken ct)
    {
        using var fs = File.OpenRead(path);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        var list = new List<string>();
        while (!sr.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();
            var line = await sr.ReadLineAsync().ConfigureAwait(false);
            if (line is not null)
            {
                list.Add(line);
            }
        }
        return list.ToArray();
    }
}
