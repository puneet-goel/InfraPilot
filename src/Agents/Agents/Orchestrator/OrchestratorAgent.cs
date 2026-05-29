using Agents.Utility;
using Agents.Workflow;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Agents.Agents.Orchestrator;

public class OrchestratorAgent
{
    private readonly IChatClient _chatClient;

    private readonly IEnumerable<IAgent> _agents;

    public OrchestratorAgent(IChatClient chatClient, IEnumerable<IAgent> agents)
    {
        _chatClient = chatClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
        _agents = agents;
    }

    public async Task<WorkflowPlan> CreatePlanAsync(string userRequest)
    {
        List<string> excludeAgents = AIHelpers.FindAgentsToExclude();

        string agentDescriptions = string.Join("\n\n", 
            _agents
            .Where(agent => !excludeAgents.Contains(agent.Name))
            .Select(agent => $$"""
            Agent: {{agent.Name}}
            Responsibilities: {{agent.Description}} 
            """));

        string prompt = $$"""
            You are an AI orchestrator.
            
            Decide which agents should execute
            the request.
                
            Available agents:

            {{agentDescriptions}}

            Return ONLY raw JSON.

            Do NOT use markdown.

            Example:
            {
              "steps": [
                {
                  "agentName":
                    "InfrastructureAgent",

                  "task":
                    "Analyze deployment health"
                }
              ]
            }

            If query is unrelated:
            {
              "steps": []
            }
            """;

        ChatOptions options = new()
        {
            Instructions = prompt
        };

        ChatResponse response =
            await _chatClient.GetResponseAsync(
                userRequest, options);

        string cleaned =
            response.Text
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

        WorkflowPlan plan =
            JsonSerializer.Deserialize<WorkflowPlan>(
                cleaned,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

        plan.RuntimeEnvironment = AIHelpers.FindRunTimeEnvironment();
        return plan ?? new WorkflowPlan();
    }
}