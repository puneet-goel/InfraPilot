using Database.Domain;

namespace Database.Repository.Interfaces;

public interface IWorkflowExecutionRepository
{
    Task<Guid> InsertWorkflowExecutionAsync(Guid workflowId);
    Task<GetWorkflowExecution?> GetWorkflowExecutionAsync(Guid workflowId);
    Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionAsync();
    Task UpdateWorkflowExecution(GetWorkflowExecution workflowExecution);
}
