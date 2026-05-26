using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCPClient;

public class DeployMcpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        bool isDevelopment = environment == "Development";

        StdioClientTransport transport =
            new (new()
            {
                Name = "Deploy Resources on Kubernetes MCP Server",
                Command = "dotnet",
                Arguments = isDevelopment
                ?
                [
                    "run",
                    "--project",
                    "../Servers/DeploymentMCP"
                ]
                :
                [
                    "/app/mcp/deployment/DeploymentMCP.dll"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}