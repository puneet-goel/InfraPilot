namespace Agents.Engine;

public interface IWorkflowEngine
{
    Task ExecuteAsync(Guid workflowId);
}