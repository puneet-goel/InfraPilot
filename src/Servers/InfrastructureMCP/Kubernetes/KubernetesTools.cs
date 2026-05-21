using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace InfrastructureMCP.Kubernetes;

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
        [Description("Kubernetes resource type")] string resourceType)
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
        [Description("Kubernetes resource type name")] string resourceType,
        [Description("Kubernetes resource name")] string resouceName)
    {
        return await RunKubectlCommand(
            $"describe {resourceType} {resouceName}");
    }

    [McpServerTool]
    [Description("Get logs for a Kubernetes pod")]
    public static async Task<string> GetPodLogs(string podName)
    {
        return await RunKubectlCommand($"logs {podName}");
    }

    [McpServerTool]
    [Description("Get Kubernetes service accounts")]
    public static async Task<string> GetServiceAccounts()
    {
        return await RunKubectlCommand($"get serviceaccounts");
    }

    [McpServerTool]
    [Description("Get pod security context configuration")]
    public static async Task<string> GetPodSecurityContext(
        [Description("Pod name")] string podName)
    {
        return await RunKubectlCommand($"get pod {podName} --o yaml");
    }

    [McpServerTool]
    [Description("Get deployment security configuration")]
    public static async Task<string> GetDeploymentSecuritySpec(
        [Description("Deployment name")] string deploymentName)
    {
        return await RunKubectlCommand($"get deployment {deploymentName} -o yaml");
    }

    [McpServerTool]
    [Description("Describe Kubernetes service account")]
    public static async Task<string> DescribeServiceAccount(
        [Description("Service account name")] string serviceAccount)
    {
        return await RunKubectlCommand($"describe serviceaccount {serviceAccount}");
    }

    [McpServerTool]
    [Description("Get Kubernetes ingresses")]
    public static async Task<string> GetIngresses()
    {
        return await RunKubectlCommand($"get ingress");
    }

    [McpServerTool]
    [Description("Get Kubernetes network policies")]
    public static async Task<string> GetNetworkPolicies()
    {
        return await RunKubectlCommand("get networkpolicies");
    }

    private static async Task<string> RunKubectlCommand(string arguments)
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