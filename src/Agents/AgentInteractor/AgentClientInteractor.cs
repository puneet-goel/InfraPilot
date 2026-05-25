using Agents.Agents;
using Microsoft.Extensions.AI;

namespace Agents.AgentInteractor;

public class AgentClientInteractor : IAgentClientInteractor
{
    private readonly IEnumerable<IAgent> _agents;

    public AgentClientInteractor(IEnumerable<IAgent> agents)
    {
        _agents = agents;
    }

    public async Task<AgentResult> ExecuteAsync(string agentName, string task, List<ChatMessage> messages)
    {
        IAgent? agent = _agents.FirstOrDefault(a => a.Name.Equals(
            agentName,
            StringComparison.OrdinalIgnoreCase));

        return agent is null
            ? throw new Exception(
                $"Unknown agent: {agentName}")
            : await agent
            .AnalyzeAsync(task, messages);
    }
}