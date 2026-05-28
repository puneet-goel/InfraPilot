using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace Agents.MCPClient;

public class DockerWriteMcpHostService
{
    public static async Task<IList<AITool>> GetToolsAsync()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        bool isDevelopment = environment == "Development";

        StdioClientTransport transport =
            new (new()
            {
                Name = "Docker with write access MCP Server",
                Command = "dotnet",
                Arguments = isDevelopment
                ?
                [
                    "run",
                    "--project",
                    "../Servers/DockerWriteMCP"
                ]
                :
                [
                    "/app/mcp/dockerwrite/DockerWriteMCP.dll"
                ]
            });

        McpClient client =
            await McpClient.CreateAsync(transport);

        IList<McpClientTool> tools =
            await client.ListToolsAsync();

        return [.. tools];
    }
}