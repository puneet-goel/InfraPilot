using Agents.Engine;
using Api.Application.Interface;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;

namespace Api.Application.Service;

public class WorkflowService: IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowService(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<CreateWorkflow> CreateWorkflowAsync(string userRequest)
    {
        CreateWorkflow workflow = await _workflowRepository
            .InsertWorkflowAsync(userRequest);

        BackgroundJob.Enqueue<IWorkflowEngine>(x => x.ExecuteAsync(workflow.Id));

        return workflow;
    }

    public async Task<GetWorkflow?> GetWorkflowAsync(string workflowId)
    {
        return await _workflowRepository.GetWorkflowAsync(Guid.Parse(workflowId));
    }

    public async Task<List<GetWorkflow>> GetAllWorkflowAsync()
    {
        return await _workflowRepository.GetAllWorkflowAsync();
    }

    public void RunWorkflow(string workflowId)
    {
        BackgroundJob.Enqueue<IWorkflowEngine>(x => x.ExecuteAsync(Guid.Parse(workflowId)));
    }
}