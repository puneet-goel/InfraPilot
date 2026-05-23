using Api.Application.Interface;
using Database.Domain;
using Database.Repository.Interfaces;

namespace Api.Application.Service;

public class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly IWorkflowExecutionRepository _workflowExecutionRepository;

    public WorkflowExecutionService(IWorkflowExecutionRepository workflowExecutionRepository)
    {
        _workflowExecutionRepository = workflowExecutionRepository;
    }

    public async Task<GetWorkflowExecution?> GetWorkflowExecutionStatusAsync(string workflowId)
    {
        return await _workflowExecutionRepository.GetWorkflowExecutionAsync(Guid.Parse(workflowId));
    }

    public async Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionStatusAsync()
    {
        return await _workflowExecutionRepository.GetAllWorkflowExecutionAsync();
    }
}