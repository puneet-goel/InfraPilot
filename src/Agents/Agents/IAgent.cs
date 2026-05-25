using Microsoft.Extensions.AI;

namespace Agents.Agents;

public interface IAgent
{
    string Name { get; }
    
    string Description { get; }

    bool IsWriteAgent { get; }

    Task<AgentResult> AnalyzeAsync(string task, List<ChatMessage> messages);
}

public class AgentResult
{
    public bool ApprovalRequired { get; set; }
    public List<ChatMessage> Messages { get; set; } = [];
}