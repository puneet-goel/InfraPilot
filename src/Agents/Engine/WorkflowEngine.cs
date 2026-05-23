using Agents.AgentInteractor;
using Agents.Agents;
using Agents.Workflow;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;
using System.Text.Json;

namespace Agents.Engine;

public class WorkflowEngine: IWorkflowEngine
{
    private readonly IAgentClientInteractor _agentClient;
    private readonly IEnumerable<IAgent> _agents;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowExecutionRepository _workflowExecutionRepository;

    public WorkflowEngine(IAgentClientInteractor agentClient, IEnumerable<IAgent> agents, IWorkflowRepository workflowRepository, IWorkflowExecutionRepository workflowExecutionRepository)
    {
        _agentClient = agentClient;
        _agents = agents;
        _workflowRepository = workflowRepository;
        _workflowExecutionRepository = workflowExecutionRepository;
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ExecuteAsync(Guid workflowId)
    {
        Guid executorId = await _workflowExecutionRepository.InsertWorkflowExecutionAsync(workflowId);
        bool error = false;

        try
        {
            GetWorkflow? workflow = await _workflowRepository.GetWorkflowAsync(workflowId);

            if (workflow == null)
            {
                throw new Exception("No workflow present with given id");
            }

            WorkflowPlan workflowPlan = JsonSerializer.Deserialize<WorkflowPlan>(workflow.Plan)!;

            string concatenatedResults = string.Empty;
            List<WorkflowStepResult> results = [];
            string result = string.Empty;

            bool isWriteAgent = workflowPlan.Steps.Any(step =>
                _agents.Any(agent =>
                agent.Name == step.AgentName && agent.IsWriteAgent));

            foreach (WorkflowStep step in workflowPlan.Steps)
            {
                result = await _agentClient.ExecuteAsync(step.AgentName, step.Task);
                results.Add(new()
                {
                    AgentName = step.AgentName,
                    Output = result
                });

                concatenatedResults += $"\n\n According to Agent: {step.AgentName} \n\n task: {step.Task} \n\n result: {result}";
                await _workflowExecutionRepository.UpdateWorkflowExecutionAgent(executorId, step.AgentName, JsonSerializer.Serialize(results));
            }

            if (!isWriteAgent)
            {
                string rootResult = await _agentClient.ExecuteAsync("RootReviewerAgent", concatenatedResults);
                results.Add(new()
                {
                    AgentName = "RootReviewerAgent",
                    Output = rootResult
                });
                await _workflowExecutionRepository.UpdateWorkflowExecutionAgent(executorId, "RootReviewerAgent", JsonSerializer.Serialize(results));
            }
        }
        catch (Exception ex)
        {
            error = true;
            await _workflowExecutionRepository.UpdateWorkflowExecutionStatus(executorId, "Failed", ex.Message);
        }
        finally
        {
            if (!error)
            {
                await _workflowExecutionRepository.UpdateWorkflowExecutionStatus(executorId, "Completed", "workflow executed successfully");
            }
        }
    }
}