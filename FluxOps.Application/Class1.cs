using FluxOps.Domain;
namespace FluxOps.Application;

/// <summary>
/// Indicates the source type for ingestion.
/// </summary>
public enum SourceType
{
	/// <summary>Inline payload provided directly.</summary>
	Inline,
	/// <summary>Local file path.</summary>
	LocalFile,
	/// <summary>Cloud object store (e.g., blob url).</summary>
	CloudObject,
	/// <summary>Event-driven input.</summary>
	Event
}

/// <summary>
/// Specification describing how data should be ingested.
/// </summary>
public sealed class IngestionSpec
{
	/// <summary>
	/// Source type.
	/// </summary>
	public SourceType SourceType { get; init; }

	/// <summary>
	/// Path or URI (for LocalFile/CloudObject).
	/// </summary>
	public string? PathOrUri { get; init; }

	/// <summary>
	/// Inline payload of raw records.
	/// </summary>
	public IReadOnlyList<string>? Payload { get; init; }
}

/// <summary>
/// Ingests data from a specified source.
/// </summary>
public interface IDataIngestion
{
	/// <summary>
	/// Ingests data based on <paramref name="spec"/>.
	/// </summary>
	Task<DataBatch> IngestAsync(IngestionSpec spec, CancellationToken cancellationToken = default);
}

/// <summary>
/// Preprocesses raw data into features.
/// </summary>
public interface IPreprocessor
{
	/// <summary>
	/// Generates a <see cref="FeatureSet"/> from the <see cref="DataBatch"/>.
	/// </summary>
	Task<FeatureSet> PreprocessAsync(DataBatch batch, CancellationToken cancellationToken = default);
}

/// <summary>
/// Executes a model against features.
/// </summary>
public interface IModelExecutor
{
	/// <summary>
	/// Produces model output for supplied features.
	/// </summary>
	Task<ModelOutput> ExecuteAsync(FeatureSet features, CancellationToken cancellationToken = default);
}

/// <summary>
/// Evaluates model output to compute metrics.
/// </summary>
public interface IEvaluator
{
	/// <summary>
	/// Computes metrics for the <see cref="ModelOutput"/>.
	/// </summary>
	Task<EvaluationResult> EvaluateAsync(ModelOutput output, CancellationToken cancellationToken = default);
}

/// <summary>
/// Routes artifacts and logs to sinks.
/// </summary>
public interface IArtifactRouter
{
	/// <summary>
	/// Routes artifacts produced by the pipeline.
	/// </summary>
	Task RouteAsync(DataBatch batch, FeatureSet features, ModelOutput output, EvaluationResult evaluation, CancellationToken cancellationToken = default);
}

/// <summary>
/// Options controlling pipeline behavior.
/// </summary>
public sealed class PipelineOptions
{
	/// <summary>
	/// Example toggle for enabling artifact routing.
	/// </summary>
	public bool EnableArtifactRouting { get; init; } = true;
}

/// <summary>
/// Orchestrates the MLOps pipeline stages.
/// </summary>
public interface IMLOpsPipeline
{
	/// <summary>
	/// Runs the end-to-end pipeline and returns evaluation results.
	/// </summary>
	Task<EvaluationResult> RunAsync(IngestionSpec spec, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of <see cref="IMLOpsPipeline"/>.
/// </summary>
public sealed class MLOpsPipeline : IMLOpsPipeline
{
	private readonly IDataIngestion _ingestion;
	private readonly IPreprocessor _preprocessor;
	private readonly IModelExecutor _modelExecutor;
	private readonly IEvaluator _evaluator;
	private readonly IArtifactRouter _artifactRouter;
	private readonly PipelineOptions _options;

	/// <summary>
	/// Creates a new <see cref="MLOpsPipeline"/>.
	/// </summary>
	public MLOpsPipeline(
		IDataIngestion ingestion,
		IPreprocessor preprocessor,
		IModelExecutor modelExecutor,
		IEvaluator evaluator,
		IArtifactRouter artifactRouter,
		PipelineOptions options)
	{
		_ingestion = ingestion;
		_preprocessor = preprocessor;
		_modelExecutor = modelExecutor;
		_evaluator = evaluator;
		_artifactRouter = artifactRouter;
		_options = options;
	}

	/// <inheritdoc />
	public async Task<EvaluationResult> RunAsync(IngestionSpec spec, CancellationToken cancellationToken = default)
	{
		var batch = await _ingestion.IngestAsync(spec, cancellationToken).ConfigureAwait(false);
		var features = await _preprocessor.PreprocessAsync(batch, cancellationToken).ConfigureAwait(false);
		var output = await _modelExecutor.ExecuteAsync(features, cancellationToken).ConfigureAwait(false);
		var eval = await _evaluator.EvaluateAsync(output, cancellationToken).ConfigureAwait(false);

		if (_options.EnableArtifactRouting)
		{
			await _artifactRouter.RouteAsync(batch, features, output, eval, cancellationToken).ConfigureAwait(false);
		}

		return eval;
	}
}

