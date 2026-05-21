using Agents.Workflow;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Agents.Agents.Orchestrator;

public class OrchestratorAgent
{
    private readonly IChatClient _chatClient;

    public OrchestratorAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<WorkflowPlan> CreatePlanAsync(
        string userRequest)
    {
        var prompt =
            $$"""
            You are an AI orchestrator. Decide which agents should execute the request on the basis of userRequest.

            Available agents:

            1. InfrastructureAgent
               - cluster
               - nodes
               - pods
               - deployments
               - configmaps
               - secrets
               - description of these resources
               - logs
               - events

            Return ONLY raw JSON.
            Do NOT use markdown.
            Do NOT wrap response in ```json.

            Example:
            {
              "steps": [
                {
                  "agentName": "InfrastructureAgent",
                  "task": "Analyze deployment health"
                }
              ]
            }

            if query is unrelated then give the response back like this
            {
              "steps": []
            } 

            User Request:
            {{userRequest}}
            """;

        ChatResponse response =
            await _chatClient.GetResponseAsync(
                prompt);

        WorkflowPlan plan =
            JsonSerializer.Deserialize<WorkflowPlan>(
                response.Text,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

        return plan ?? new WorkflowPlan();
    }
}

//2. SecurityAgent
//               - deployment security
//               - insecure configurations
//               - missing probes
//               - privileged containers