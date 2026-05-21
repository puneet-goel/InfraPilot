using Agents.AgentClientInteractor;

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
        List<string> results = [];

        foreach (WorkflowStep step in workflowPlan.Steps)
        {
            string context = results.Count == 0 ? string.Empty : results.Last();
            string result = await _agentClient.ExecuteAsync(step.AgentName, step.Task, context);
            results.Add(result);
        }

        return (results.Count == 0 ? string.Empty : results.Last());
    }
}