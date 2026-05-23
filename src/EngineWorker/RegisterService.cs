using EngineWorker.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace EngineWorker.RegisterServices;

public static class EngineServiceExtensions
{
    public static IServiceCollection AddEngine(this IServiceCollection services)
    {
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();

        return services;
    }
}
