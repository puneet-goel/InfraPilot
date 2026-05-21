using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCP;

public class McpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        StdioClientTransport transport =
            new (new()
            {
                Name = "Kubernetes MCP Server",

                Command = "dotnet",

                Arguments =
                [
                    "run",
                    "--project",
                    "../MCPServer"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}