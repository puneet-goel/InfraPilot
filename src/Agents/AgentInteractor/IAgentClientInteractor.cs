namespace Agents.AgentInteractor;

public interface IAgentClientInteractor
{
    Task<string> ExecuteAsync(string agentName, string task);
}