using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCPClient;

public class DockerReadMcpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        bool isDevelopment = environment == "Development";

        StdioClientTransport transport =
            new (new()
            {
                Name = "Docker with read access MCP Server",
                Command = "dotnet",
                Arguments = isDevelopment
                ?
                [
                    "run",
                    "--project",
                    "../Servers/DockerReadMCP"
                ]
                :
                [
                    "/app/mcp/dockerread/DockerReadMCP.dll"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}