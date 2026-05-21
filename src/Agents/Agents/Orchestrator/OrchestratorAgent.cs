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
        _chatClient = chatClient;
        _agents = agents;
    }

    public async Task<WorkflowPlan> CreatePlanAsync(string userRequest)
    {
        string agentDescriptions = string.Join("\n\n", 
            _agents
            .Where(agent =>
                agent.Name !=
                "RootReviewerAgent")
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

            User Request:
            {{userRequest}}
            """;

        ChatResponse response =
            await _chatClient.GetResponseAsync(
                prompt);

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

        return plan ?? new WorkflowPlan();
    }
}