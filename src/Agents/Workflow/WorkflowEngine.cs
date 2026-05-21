using Agents.AgentInteractor;

namespace Agents.Workflow;

public class WorkflowEngine
{
    private readonly IAgentClientInteractor _agentClient;

    public WorkflowEngine(IAgentClientInteractor agentClient)
    {
        _agentClient = agentClient;
    }

    public async Task<string> ExecuteAsync(WorkflowPlan workflowPlan)
    {
        string concatenatedResults = string.Empty;

        foreach (WorkflowStep step in workflowPlan.Steps)
        {
            string result = await _agentClient.ExecuteAsync(step.AgentName, step.Task);
            concatenatedResults += $"\n\n According to Agent: {step.AgentName} \n\n task: {step.Task} \n\n result: {result}";
        }

        string rootResult = await _agentClient.ExecuteAsync("RootReviewerAgent", concatenatedResults);
        return rootResult;
    }
}