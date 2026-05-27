using Agents.AgentInteractor;
using Agents.Agents;
using Agents.Agents.Orchestrator;
using Agents.Utility;
using Agents.Workflow;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Agents.Engine;

public class WorkflowEngine(
    IAgentClientInteractor agentClient,
    IEnumerable<IAgent> agents,
    IWorkflowExecutionRepository workflowExecutionRepository,
    OrchestratorAgent orchestratorAgent
) : IWorkflowEngine
{
    private readonly IAgentClientInteractor _agentClient = agentClient;
    private readonly IEnumerable<IAgent> _agents = agents;
    private readonly OrchestratorAgent _orchestratorAgent = orchestratorAgent;
    private readonly IWorkflowExecutionRepository _workflowExecutionRepository = workflowExecutionRepository;

    [AutomaticRetry(Attempts = 0)]
    public async Task ExecuteAsync(Guid executorId)
    {
        // fetch workflow details
        GetWorkflowExecution? workflowExecution = await _workflowExecutionRepository.GetWorkflowExecutionAsync(executorId);
        if(workflowExecution == null)
        {
            return;
        }

        try
        {
            // not adding Failed scenario if someone wants to re-run
            List<string> codesToStopExecution = ["Rejected", "ApprovalRequired"];
            if (codesToStopExecution.Contains(workflowExecution.Status!))
            {
               return;
            }

            // workflow started
            workflowExecution.CurrentAgent ??= "OrchestratorAgent";
            workflowExecution.Status = "Running";
            await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);

            // construct plan
            WorkflowPlanResult? lastOutputSaved = null;
            WorkflowPlan workflowPlan = new();
            int startIndex = 0;

            if (workflowExecution.AgentOutput != null)
            {
                lastOutputSaved = JsonSerializer.Deserialize<WorkflowPlanResult>(workflowExecution.AgentOutput)!;
            }

            // resume state Human in the loop
            if (lastOutputSaved != null)
            {
                workflowPlan = JsonSerializer.Deserialize<WorkflowPlan>(workflowExecution.WorkflowPlan!) ?? new();
                startIndex = workflowPlan.Steps.FindIndex(ele => ele.AgentName == workflowExecution.CurrentAgent);

                if(startIndex == -1)
                {
                    startIndex = workflowPlan.Steps.Count;
                }
            }
            else
            {
                // first step generate plan and save in db
                workflowPlan = await _orchestratorAgent.CreatePlanAsync(workflowExecution.UserRequest);

                // modify plan for user interface
                workflowPlan.Steps.Insert(0, new WorkflowStep()
                {
                    Task = "Generate a workflow/plan for the user request.",
                    AgentName = "OrchestratorAgent"
                });

                bool isWriteAgent = workflowPlan.Steps.Any(step =>
                    _agents.Any(agent =>
                    agent.Name == step.AgentName && agent.IsWriteAgent));

                if (!isWriteAgent)
                {
                    workflowPlan.Steps.Add(new WorkflowStep()
                    {
                        Task = "Analyse the final findings.",
                        AgentName = "RootReviewerAgent"
                    });
                }

                // update plan in db
                workflowExecution.WorkflowPlan = JsonSerializer.Serialize(workflowPlan);
                await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);
            }

            // plan is ready at this stage
            WorkflowPlanResult results = new()
            {
                RuntimeEnvironment = workflowPlan.RuntimeEnvironment,
            };
            string concatenatedResults = string.Empty;

            for(int i = startIndex; i < workflowPlan.Steps.Count; ++i)
            {
                WorkflowStep step = workflowPlan.Steps[i];
                workflowExecution.CurrentAgent = step.AgentName;
                AgentResult agentResponse = new();

                if (step.AgentName == "OrchestratorAgent")
                {
                    continue;
                }

                await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);

                if (step.AgentName == "RootReviewerAgent")
                {
                    concatenatedResults += $"\n\n User Query: {workflowExecution.UserRequest}";
                    agentResponse = await _agentClient.ExecuteAsync(step.AgentName, concatenatedResults, []);
                }
                else
                {
                    // take out previous history in case of resume
                    List<ChatMessage> prevMessages = lastOutputSaved == null
                        ? []
                        : AIHelpers.ConvertToAgentResult(lastOutputSaved.Steps.First(ele => ele.AgentName == step.AgentName));

                    agentResponse = await _agentClient.ExecuteAsync(step.AgentName, step.Task, prevMessages);
                }

                // convert agent response to db compatible
                AgentOutput agentOutput = AIHelpers.ConvertToAgentOutput(agentResponse, step.AgentName);

                if (agentResponse.ApprovalRequired) {
                    agentOutput.Chat[^1].ApprovalStatus = "Pending";
                    agentOutput.Chat[^1].IsApprovalRequired = true;
                
                    results.Steps.Add(agentOutput);

                    workflowExecution.AgentOutput = JsonSerializer.Serialize(results);
                    workflowExecution.Status = "ApprovalRequired";
                    await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);
                    return;
                }

                results.Steps.Add(agentOutput);
                concatenatedResults += $"\n\n According to Agent: {step.AgentName} \n\n task: {step.Task} \n\n result: {agentResponse.Messages.Last().Text}";
   
                workflowExecution.AgentOutput = JsonSerializer.Serialize(results);
                await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);
            }

            workflowExecution.Reason = "Workflow executed successfully";
            workflowExecution.Status = "Completed";
            await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);
        }
        catch (Exception ex)
        {
            workflowExecution.Reason = ex.Message;
            workflowExecution.Status = "Failed";
            await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);
        }
    }
}