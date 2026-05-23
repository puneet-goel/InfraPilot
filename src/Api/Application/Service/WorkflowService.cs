using Agents.Agents.Orchestrator;
using Agents.Engine;
using Agents.Workflow;
using Api.Application.Interface;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;
using System.Text.Json;

namespace Api.Application.Service;

public class WorkflowService: IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly OrchestratorAgent _orchestratorAgent;

    public WorkflowService(IWorkflowRepository workflowRepository, OrchestratorAgent orchestratorAgent)
    {
        _workflowRepository = workflowRepository;
        _orchestratorAgent = orchestratorAgent;
    }

    public async Task<CreateWorkflow> CreateWorkflowAsync(string userRequest)
    {
        WorkflowPlan plan = await _orchestratorAgent.CreatePlanAsync(userRequest);

        CreateWorkflow workflow = await _workflowRepository
            .InsertWorkflowAsync(userRequest, JsonSerializer.Serialize(plan));

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