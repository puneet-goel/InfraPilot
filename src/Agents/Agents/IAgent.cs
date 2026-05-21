namespace Agents.Agents;

public interface IAgent
{
    string Name { get; }
    
    string Description { get; }

    bool IsWriteAgent { get; }

    Task<string> AnalyzeAsync(string task);
}