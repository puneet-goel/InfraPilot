using Database.Domain;

namespace Api.Application.Interface;

public interface IWorkflowService
{
    Task<CreateWorkflow> CreateWorkflowAsync(string userRequest);
    Task<GetWorkflow?> GetWorkflowAsync(string workflowId);
    Task<List<GetWorkflow>> GetAllWorkflowAsync();
    void RunWorkflow(string workflowId);
}