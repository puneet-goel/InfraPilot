using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCP;

public class DeployMcpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        StdioClientTransport transport =
            new (new()
            {
                Name = "Deploy Resources on Kubernetes MCP Server",

                Command = "dotnet",

                Arguments =
                [
                    "run",
                    "--project",
                    "../Server/DeploymentMCP"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}