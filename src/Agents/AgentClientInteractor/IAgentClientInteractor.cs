namespace Agents.AgentClientInteractor;

public interface IAgentClientInteractor
{
    Task<string> ExecuteAsync(string agentName, string task);
}