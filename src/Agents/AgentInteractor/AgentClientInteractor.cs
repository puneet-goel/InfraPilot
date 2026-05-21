using Agents.Agents;

namespace Agents.AgentInteractor;

public class AgentClientInteractor : IAgentClientInteractor
{
    private readonly IEnumerable<IAgent> _agents;

    public AgentClientInteractor(IEnumerable<IAgent> agents)
    {
        _agents = agents;
    }

    public async Task<string> ExecuteAsync(string agentName, string task)
    {
        IAgent? agent = _agents.FirstOrDefault(a => a.Name.Equals(
            agentName,
            StringComparison.OrdinalIgnoreCase));

        return agent is null
            ? throw new Exception(
                $"Unknown agent: {agentName}")
            : await agent
            .AnalyzeAsync(task);
    }
}