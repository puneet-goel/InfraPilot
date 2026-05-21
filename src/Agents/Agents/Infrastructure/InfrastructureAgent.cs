using Agents.MCP;
using Microsoft.Extensions.AI;

namespace Agents.Agents.Infrastructure;

public class InfrastructureAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public InfrastructureAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public string Name => "InfrastructureAgent";

    public string Description =>
    """
    Handles:
    - cluster analysis
    - nodes
    - pods
    - deployments
    - logs
    - events
    - configmaps
    - secrets
    """;

    public async Task<string> AnalyzeAsync(string query)
    {
        IList<AITool> tools = await InfraMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools
        };

        ChatResponse response =
            await _chatClient.GetResponseAsync(
                $""""
                You are an expert Kubernetes SRE engineer.

                Always investigate unhealthy pods thoroughly.

                Use multiple tools if needed before answering.

                Query: {query}
                """",
                options);

        return response.Text;
    }
}