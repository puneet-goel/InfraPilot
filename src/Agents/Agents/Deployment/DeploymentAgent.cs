using Agents.MCP;
using Microsoft.Extensions.AI;

namespace Agents.Agents.Deployment;

public class DeploymentAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public DeploymentAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public string Name => "DeploymentAgent";

    public bool IsWriteAgent => true;

    public string Description =>
    """
    Handles:
    - deployment updates
    - YAML generation
    - rollout restarts
    - scaling deployments
    - applying manifests
    - patching workloads
    """;

    public async Task<string> AnalyzeAsync(string query)
    {
        IList<AITool> tools = await DeployMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools
        };

        ChatResponse response =
            await _chatClient.GetResponseAsync(
               $$"""
                You are a Kubernetes infrastructure provisioning expert who deploy resources.

                Your responsibilities:
                - generate Kubernetes manifests
                - apply infrastructure changes
                - restart deployments
                - scale workloads
                - update deployment images

                ONLY perform actions requested.

                Be safe and conservative.

                Request:
                {{query}}
                """,
            options);

        return response.Text;
    }
}
