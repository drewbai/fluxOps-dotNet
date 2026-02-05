using FluxOps.Application;
using FluxOps.Infrastructure.Ingestion;
using FluxOps.Infrastructure.Preprocessing;
using FluxOps.Infrastructure.Model;
using FluxOps.Infrastructure.Evaluation;
using FluxOps.Infrastructure.Artifacts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluxOps.Infrastructure;

/// <summary>
/// Service registration helpers for FluxOps.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers FluxOps pipeline services and binds options.
    /// </summary>
    public static IServiceCollection AddFluxOps(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new PipelineOptions();
        configuration.GetSection("Pipeline").Bind(options);

        services.AddSingleton(options);

        services.AddScoped<IDataIngestion, DefaultDataIngestion>();
        services.AddScoped<IPreprocessor, SimplePreprocessor>();
        services.AddScoped<IModelExecutor, MockModelExecutor>();
        services.AddScoped<IEvaluator, BasicEvaluator>();
        services.AddScoped<IArtifactRouter, ConsoleArtifactRouter>();
        services.AddScoped<IMLOpsPipeline, MLOpsPipeline>();

        return services;
    }
}
