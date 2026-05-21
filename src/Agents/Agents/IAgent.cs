namespace Agents.Agents;

public interface IAgent
{
    string Name { get; }
    string Description { get; }
    Task<string> AnalyzeAsync(string task);
}