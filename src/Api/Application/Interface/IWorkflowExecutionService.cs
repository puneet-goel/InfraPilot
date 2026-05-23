using Database.Domain;

namespace Api.Application.Interface;

public interface IWorkflowExecutionService
{
    Task<GetWorkflowExecution?> GetWorkflowExecutionStatusAsync(string workflowId);
    Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionStatusAsync();
}