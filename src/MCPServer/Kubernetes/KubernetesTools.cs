using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace MCPServer.Kubernetes;

[McpServerToolType]
public static class KubernetesTools
{
    [McpServerTool, Description("Get Kubernetes pods")]
    public static async Task<string> GetPods()
    {
        ProcessStartInfo processInfo = new()
        {
            FileName = "kubectl",
            Arguments = "get pods",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new()
        {
            StartInfo = processInfo
        };

        process.Start();

        string output =
            await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return output;
    }
}