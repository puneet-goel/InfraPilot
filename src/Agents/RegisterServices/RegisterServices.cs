using Agents.AgentInteractor;
using Agents.Agents;
using Agents.Agents.Deployment;
using Agents.Agents.DockerRead;
using Agents.Agents.DockerWrite;
using Agents.Agents.Infrastructure;
using Agents.Agents.Orchestrator;
using Agents.Agents.RootReviewer;
using Agents.Engine;
using Agents.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.RegisterServices;

public static class AgentServiceExtensions
{
    public static IServiceCollection AddAgents(this IServiceCollection services)
    {
        services.AddScoped<IAgent, KubernetesDeploymentAgent>();
        services.AddScoped<IAgent, RootReviewerAgent>();
        services.AddScoped<IAgent, KubernetesInfrastructureAgent>();
        services.AddScoped<IAgent, DockerReadAgent>();
        services.AddScoped<IAgent, DockerWriteAgent>();
        services.AddScoped<OrchestratorAgent>();
        services.AddScoped<IAgentClientInteractor, AgentClientInteractor>();
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();
        services.AddSingleton<WorkflowEventBus>();

        return services;
    }
}