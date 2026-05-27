using Agents.Engine;
using Api.Application.DTO;
using Api.Application.Interface;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;

namespace Api.Application.Service;

public class WorkflowService: IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowExecutionRepository _workflowExecutionRepository;

    public WorkflowService(IWorkflowRepository workflowRepository, IWorkflowExecutionRepository workflowExecutionRepository)
    {
        _workflowRepository = workflowRepository;
        _workflowExecutionRepository = workflowExecutionRepository;
    }

    public async Task<CreateWorkflow> CreateWorkflowAsync(string userRequest)
    {
        CreateWorkflow workflow = await _workflowRepository
            .InsertWorkflowAsync(userRequest);

        Guid executionId = await _workflowExecutionRepository
            .InsertWorkflowExecutionAsync(workflow.Id);

        BackgroundJob.Enqueue<IWorkflowEngine>(x => x.ExecuteAsync(executionId));

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

    public async Task<Guid> RunWorkflow(ReRunWorkflow req)
    {
        Guid executionId = Guid.Parse(req.ExecutionId);
        if (!req.UseSamePlan)
        {
            GetWorkflow? workflow = await _workflowRepository.GetWorkflowAsync(Guid.Parse(req.WorkflowId));

            if (workflow == null)
            {
                throw new Exception($"No workflow present with given {req.WorkflowId}");
            }

            executionId = await _workflowExecutionRepository
                .InsertWorkflowExecutionAsync(workflow.Id);
        }

        BackgroundJob.Enqueue<IWorkflowEngine>(x => x.ExecuteAsync(executionId));
        return executionId;
    }
}