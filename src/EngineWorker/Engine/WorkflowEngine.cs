using Agents.AgentInteractor;
using Agents.Agents;
using Agents.Workflow;

namespace EngineWorker.Engine;

public class WorkflowEngine: IWorkflowEngine
{
    private readonly IAgentClientInteractor _agentClient;
    private readonly IEnumerable<IAgent> _agents;

    public WorkflowEngine(IAgentClientInteractor agentClient, IEnumerable<IAgent> agents)
    {
        _agentClient = agentClient;
        _agents = agents;
    }

    public async Task<string> ExecuteAsync(WorkflowPlan workflowPlan)
    {
        string concatenatedResults = string.Empty;
        bool isWriteAgent = workflowPlan.Steps.Any(step => 
            _agents.Any(agent => 
            agent.Name == step.AgentName && agent.IsWriteAgent));
        string result = string.Empty;

        foreach (WorkflowStep step in workflowPlan.Steps)
        {
            result = await _agentClient.ExecuteAsync(step.AgentName, step.Task);
            concatenatedResults += $"\n\n According to Agent: {step.AgentName} \n\n task: {step.Task} \n\n result: {result}";
        }

        if (!isWriteAgent)
        {
            string rootResult = await _agentClient.ExecuteAsync("RootReviewerAgent", concatenatedResults);
            return rootResult;
        }

        return result;
    }
}