using Agents.Infrastructure;

namespace Agents.AgentClientInteractor;

public class AgentClientInteractor : IAgentClientInteractor
{
    private readonly InfrastructureAgent _infraAgent;

    public AgentClientInteractor(InfrastructureAgent infraAgent)
    {
        _infraAgent = infraAgent;
    }

    public async Task<string> ExecuteAsync(string agentName, string task, string context)
    {
        string result =
            agentName switch
            {
                "InfrastructureAgent" =>
                   await _infraAgent.AnalyzeClusterAsync(task, context),

                _ => throw new Exception(
                    $"Unknown agent: {agentName}")
            };

        return result;
    }
}