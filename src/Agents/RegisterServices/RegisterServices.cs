using Agents.AgentInteractor;
using Agents.Agents;
using Agents.Agents.Deployment;
using Agents.Agents.Infrastructure;
using Agents.Agents.Orchestrator;
using Agents.Agents.RootReviewer;
using Agents.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.RegisterServices;

public static class AgentServiceExtensions
{
    public static IServiceCollection AddAgents(this IServiceCollection services)
    {
        services.AddScoped<IAgent, DeploymentAgent>();
        services.AddScoped<IAgent, RootReviewerAgent>();
        services.AddScoped<IAgent, InfrastructureAgent>();
        services.AddScoped<OrchestratorAgent>();
        services.AddScoped<IAgentClientInteractor, AgentClientInteractor>();
        services.AddScoped<WorkflowEngine>();

        return services;
    }
}