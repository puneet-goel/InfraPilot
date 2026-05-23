using Database.Domain;

namespace Database.Repository.Interfaces;

public interface IWorkflowRepository
{
    Task<CreateWorkflow> InsertWorkflowAsync(string userRequest, string plan);

    Task<GetWorkflow?> GetWorkflowAsync(Guid workflowId);
    Task<List<GetWorkflow>> GetAllWorkflowAsync();
}