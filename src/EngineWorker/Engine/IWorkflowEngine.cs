namespace EngineWorker;

public interface IWorkflowEngine
{
    Task ExecuteAsync(Guid workflowId);
}