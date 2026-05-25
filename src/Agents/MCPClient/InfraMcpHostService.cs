using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCPClient;

public class InfraMcpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        StdioClientTransport transport =
            new (new()
            {
                Name = "Read Kubernetes MCP Server",

                Command = "dotnet",

                Arguments =
                [
                    "/app/mcp/infrastructure/InfrastructureMCP.dll"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}