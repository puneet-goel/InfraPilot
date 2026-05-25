using Agents.Agents;
using Microsoft.Extensions.AI;

namespace Agents.AgentInteractor;

public interface IAgentClientInteractor
{
    Task<AgentResult> ExecuteAsync(string agentName, string task, List<ChatMessage> messages);
}