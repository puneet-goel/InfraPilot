using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace InfrastructureMCP.Kubernetes;

[McpServerToolType]
public class KubernetesTools
{
    private readonly ILogger<KubernetesTools> _logger;

    public KubernetesTools(ILogger<KubernetesTools> logger)
    {
        _logger = logger;
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
    public async Task<string> GetResource(
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
    public async Task<string> DescribeSpecificResource(
        [Description("Kubernetes resource type name")] string resourceType,
        [Description("Kubernetes resource name")] string resouceName)
    {
        return await RunKubectlCommand(
            $"describe {resourceType} {resouceName}");
    }

    [McpServerTool]
    [Description("Get logs for a Kubernetes pod")]
    public async Task<string> GetPodLogs(string podName)
    {
        return await RunKubectlCommand($"logs {podName}");
    }

    [McpServerTool]
    [Description("Get Kubernetes service accounts")]
    public async Task<string> GetServiceAccounts()
    {
        return await RunKubectlCommand($"get serviceaccounts");
    }

    [McpServerTool]
    [Description("Get pod security context configuration")]
    public async Task<string> GetPodSecurityContext(
        [Description("Pod name")] string podName)
    {
        return await RunKubectlCommand($"get pod {podName} --o yaml");
    }

    [McpServerTool]
    [Description("Get deployment security configuration")]
    public async Task<string> GetDeploymentSecuritySpec(
        [Description("Deployment name")] string deploymentName)
    {
        return await RunKubectlCommand($"get deployment {deploymentName} -o yaml");
    }

    [McpServerTool]
    [Description("Describe Kubernetes service account")]
    public async Task<string> DescribeServiceAccount(
        [Description("Service account name")] string serviceAccount)
    {
        return await RunKubectlCommand($"describe serviceaccount {serviceAccount}");
    }

    [McpServerTool]
    [Description("Get Kubernetes ingresses")]
    public async Task<string> GetIngresses()
    {
        return await RunKubectlCommand($"get ingress");
    }

    [McpServerTool]
    [Description("Get Kubernetes network policies")]
    public async Task<string> GetNetworkPolicies()
    {
        return await RunKubectlCommand("get networkpolicies");
    }

    private async Task<string> RunKubectlCommand(string arguments)
    {
        string executionId = Guid.NewGuid().ToString()[..8];

        Stopwatch stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            """
            [{ExecutionId}] Starting kubectl command.
            Arguments: {Arguments}
            Timestamp: {Timestamp}
            """,
            executionId,
            arguments,
            DateTime.UtcNow);

        ProcessStartInfo processInfo = new()
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
            StartInfo = processInfo
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        stopwatch.Stop();

        _logger.LogInformation(
            """
            [{ExecutionId}] Kubectl command completed.
            ExitCode: {ExitCode}
            DurationMs: {Duration}
            """,
            executionId,
            process.ExitCode,
            stopwatch.ElapsedMilliseconds);

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogError(
                """
                [{ExecutionId}] Kubectl command failed.
                Error: {Error}
                """,
                executionId,
                error);
        }
        else
        {
            _logger.LogInformation(
                """
                [{ExecutionId}] Kubectl command succeeded.
                OutputLength: {OutputLength}
                """,
                executionId,
                output.Length);
        }

        return string.IsNullOrWhiteSpace(error)
            ? output
            : error;
    }
}