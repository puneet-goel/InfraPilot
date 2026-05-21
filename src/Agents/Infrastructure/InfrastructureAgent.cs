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

    public async Task<string> AnalyzeClusterAsync(string query, string context)
    {
        IList<AITool> tools = await McpHostService.GetToolsAsync();

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

                Context: {context}
                """",
                options);

        return response.Text;
    }
}