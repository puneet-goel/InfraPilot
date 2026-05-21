using Agents.Agents.Infrastructure;
using Agents.Agents.RootReviewer;

namespace Agents.AgentInteractor;

public class AgentClientInteractor : IAgentClientInteractor
{
    private readonly InfrastructureAgent _infraAgent;
    private readonly RootReviewerAgent _rootReviewerAgent;

    public AgentClientInteractor(InfrastructureAgent infraAgent, RootReviewerAgent rootReviewerAgent)
    {
        _infraAgent = infraAgent;
        _rootReviewerAgent = rootReviewerAgent;
    }

    public async Task<string> ExecuteAsync(string agentName, string task)
    {
        string result =
            agentName switch
            {
                "InfrastructureAgent" =>
                   await _infraAgent.AnalyzeAsync(task),

                "RootReviewer" =>
                   await _rootReviewerAgent.AnalyzeAsync(task),

                _ => throw new Exception(
                    $"Unknown agent: {agentName}")
            };

        return result;
    }
}