using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace MCPServer.Kubernetes;

[McpServerToolType]
public static class KubernetesTools
{
    [McpServerTool]
    [Description("""
        Get Kubernetes resources.
        
        Valid resource types:
        - pods
        - deployments
        - events
        - nodes
        - configmaps
        - secrets
        - svc (services)
    """)]
    public static async Task<string> GetResource(
    [Description("Kubernetes resource type")]
    string resourceType)
    {
        return await RunKubectlCommand(
            $"get {resourceType}");
    }

    [McpServerTool]
    [Description("""
        Get Kubernetes resources.
        
        Valid resource types:
        - pods
        - deployments
        - events
        - nodes
        - configmaps
        - secrets
        - svc (services)
    """)]
    public static async Task<string> DescribeSpecificResource(
        [Description("Kubernetes resource type")]
        string resourceType,
        [Description("Kubernetes resource type")]
        string resouceName)
    {
        return await RunKubectlCommand(
            $"describe {resourceType} {resouceName}");
    }

    [McpServerTool]
    [Description("Get logs for a Kubernetes pod")]
    public static async Task<string> GetPodLogs(
        string podName)
    {
        return await RunKubectlCommand(
            $"logs {podName}");
    }

    private static async Task<string> RunKubectlCommand(
        string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "kubectl",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = processInfo
        };

        process.Start();

        var output =
            await process.StandardOutput
                .ReadToEndAsync();

        var error =
            await process.StandardError
                .ReadToEndAsync();

        await process.WaitForExitAsync();

        return string.IsNullOrWhiteSpace(error)
            ? output
            : error;
    }
}