using Api.Application.DTO;
using Database.Domain;

namespace Api.Application.Interface;

public interface IWorkflowService
{
    Task<CreateWorkflow> CreateWorkflowAsync(string userRequest);
    Task<GetWorkflow?> GetWorkflowAsync(string workflowId);
    Task<List<GetWorkflow>> GetAllWorkflowAsync();
    Task<Guid> RunWorkflow(ReRunWorkflow req);
}