using Agents.MCP;
using Microsoft.Extensions.AI;

namespace Agents.Infrastructure;

public class InfrastructureAgent
{
    private readonly IChatClient _chatClient;

    public InfrastructureAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> AnalyzeClusterAsync()
    {
        IList<AITool> tools =
            await McpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools
        };

        ChatResponse response =
            await _chatClient.GetResponseAsync(
                """
                Analyze my Kubernetes cluster.
                Use tools if needed.
                """,
                options);

        return response.Text;
    }
}