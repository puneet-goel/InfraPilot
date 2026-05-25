using Api.Application.DTO;
using Database.Domain;

namespace Api.Application.Interface;

public interface IWorkflowExecutionService
{
    Task<GetWorkflowExecution?> GetWorkflowExecutionStatusAsync(string executionId);
    Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionStatusAsync();
    Task AcceptWorkflowExecution(AcceptWorkflowExecution req);
}