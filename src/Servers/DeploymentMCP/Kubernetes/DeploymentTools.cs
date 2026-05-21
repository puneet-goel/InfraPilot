using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace DeploymentMCP.Kubernetes;

[McpServerToolType]
public static class DeploymentTools
{
    [McpServerTool]
    [Description("Apply Kubernetes YAML manifest")]
    public static async Task<string> ApplyYaml(
        [Description("Path to YAML file")] string filePath)
    {
        return await RunKubectlCommand($"apply -f {filePath}");
    }

    [McpServerTool]
    [Description("Restart deployment rollout")]
    public static async Task<string> RestartDeployment(
        [Description("Deployment name")] string deploymentName,
        [Description("Namespace")] string ns = "default")
    {
        return await RunKubectlCommand(
            $"rollout restart deployment " +
            $"{deploymentName} -n {ns}");
    }

    [McpServerTool]
    [Description("Scale deployment replicas")]
    public static async Task<string> ScaleDeployment(
        [Description("Deployment name")] string deploymentName,
        [Description("Replica count")] int replicas,
        [Description("Namespace")] string ns = "default")
    {
        return await RunKubectlCommand(
            $"scale deployment " +
            $"{deploymentName} " +
            $"--replicas={replicas} " +
            $"-n {ns}");
    }

    [McpServerTool]
    [Description("Patch deployment image")]
    public static async Task<string> UpdateDeploymentImage(
        [Description("Deployment name")] string deploymentName,
        [Description("Container name")] string containerName,
        [Description("New image")] string image,
        [Description("Namespace")] string ns = "default")
    {
        return await RunKubectlCommand(
            $"set image deployment/" +
            $"{deploymentName} " +
            $"{containerName}={image} " +
            $"-n {ns}");
    }

    private static async Task<string> RunKubectlCommand(string arguments)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "kubectl",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new()
        {
            StartInfo = startInfo
        };

        process.Start();

        string output =
            await process.StandardOutput
                .ReadToEndAsync();

        string error =
            await process.StandardError
                .ReadToEndAsync();

        await process.WaitForExitAsync();

        return string.IsNullOrWhiteSpace(error)
            ? output
            : error;
    }
}