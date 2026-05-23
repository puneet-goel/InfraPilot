using Database.Domain;

namespace Database.Repository.Interfaces;

public interface IWorkflowExecutionRepository
{
    Task<Guid> InsertWorkflowExecutionAsync(Guid workflowId);
    Task<GetWorkflowExecution?> GetWorkflowExecutionAsync(Guid workflowId);
    Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionAsync();
    Task UpdateWorkflowExecutionStatus(Guid executionId, string staus, string? reason);
    Task UpdateWorkflowExecutionAgent(Guid executionId, string currentAgent, string agentOutput);    
}
