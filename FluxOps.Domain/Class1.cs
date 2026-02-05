namespace FluxOps.Domain;

/// <summary>
/// Represents a batch of raw data ingested from a source.
/// </summary>
public sealed class DataBatch
{
	/// <summary>
	/// The source identifier or URI for the batch.
	/// </summary>
	public string Source { get; }

	/// <summary>
	/// Timestamp when the batch was ingested.
	/// </summary>
	public DateTimeOffset Timestamp { get; }

	/// <summary>
	/// Raw records for this batch.
	/// </summary>
	public IReadOnlyList<string> Records { get; }

	/// <summary>
	/// Creates a new <see cref="DataBatch"/>.
	/// </summary>
	public DataBatch(string source, DateTimeOffset timestamp, IReadOnlyList<string> records)
	{
		Source = source;
		Timestamp = timestamp;
		Records = records;
	}
}

/// <summary>
/// Represents a set of features produced by preprocessing.
/// </summary>
public sealed class FeatureSet
{
	/// <summary>
	/// Feature vector values.
	/// </summary>
	public IReadOnlyList<double> Features { get; }

	/// <summary>
	/// Creates a new <see cref="FeatureSet"/>.
	/// </summary>
	public FeatureSet(IReadOnlyList<double> features)
	{
		Features = features;
	}
}

/// <summary>
/// Output produced by a model execution.
/// </summary>
public sealed class ModelOutput
{
	/// <summary>
	/// Model prediction values.
	/// </summary>
	public IReadOnlyList<double> Predictions { get; }

	/// <summary>
	/// Optional metadata.
	/// </summary>
	public string? Metadata { get; }

	/// <summary>
	/// Creates a new <see cref="ModelOutput"/>.
	/// </summary>
	public ModelOutput(IReadOnlyList<double> predictions, string? metadata = null)
	{
		Predictions = predictions;
		Metadata = metadata;
	}
}

/// <summary>
/// Metrics and results from evaluation.
/// </summary>
public sealed class EvaluationResult
{
	/// <summary>
	/// Indicates whether evaluation succeeded.
	/// </summary>
	public bool Succeeded { get; }

	/// <summary>
	/// Metrics produced by evaluation.
	/// </summary>
	public IReadOnlyDictionary<string, double> Metrics { get; }

	/// <summary>
	/// Optional message or notes.
	/// </summary>
	public string? Message { get; }

	/// <summary>
	/// Creates a new <see cref="EvaluationResult"/>.
	/// </summary>
	public EvaluationResult(bool succeeded, IReadOnlyDictionary<string, double> metrics, string? message = null)
	{
		Succeeded = succeeded;
		Metrics = metrics;
		Message = message;
	}
}

