using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;

namespace DeploymentMCP.Kubernetes;

[McpServerToolType]
public class DeploymentTools
{
    private readonly ILogger<DeploymentTools> _logger;

    public DeploymentTools(ILogger<DeploymentTools> logger)
    {
        _logger = logger;
    }

    [McpServerTool]
    [Description("Create Kubernetes deployment")]
    public async Task<string> CreateDeployment(
         [Description("Deployment name")] string name,
         [Description("Container image")] string image,
         [Description("Replica count")] int replicas,
         [Description("Container port")] int port,
         [Description("Namespace")] string ns = "default")
    {
        var yaml =
            $$"""
            apiVersion: apps/v1
            kind: Deployment
            metadata:
              name: {{name}}
              namespace: {{ns}}
            spec:
              replicas: {{replicas}}
              selector:
                matchLabels:
                  app: {{name}}
              template:
                metadata:
                  labels:
                    app: {{name}}
                spec:
                  containers:
                  - name: {{name}}
                    image: {{image}}
                    ports:
                    - containerPort: {{port}}
            """;

        string tempFile = await CreateYamlFile(yaml);

        await File.WriteAllTextAsync(
            tempFile,
            yaml);

        return await RunKubectlCommand(
            $"apply -f {tempFile}");
    }

    [McpServerTool]
    [Description("Create Kubernetes namespace")]
    public async Task<string> CreateNamespace(
        [Description("Namespace name")] string name)
    {
        var yaml =
            $$"""
            apiVersion: v1
            kind: Namespace
            metadata:
              name: {{name}}
            """;

        string tempFile = await CreateYamlFile(yaml);

        return await RunKubectlCommand(
            $"apply -f {tempFile}");
    }

    [McpServerTool]
    [Description("Create Kubernetes pod")]
    public async Task<string> CreatePod(
        [Description("Pod name")] string name,
        [Description("Container image")] string image,
        [Description("Container port")] int port,
        [Description("Namespace")] string ns = "default")
    {
        var yaml =
            $$"""
            apiVersion: v1
            kind: Pod
            metadata:
              name: {{name}}
              namespace: {{ns}}
            spec:
              containers:
              - name: {{name}}
                image: {{image}}
                ports:
                - containerPort: {{port}}
            """;

        string tempFile = await CreateYamlFile(yaml);

        return await RunKubectlCommand(
            $"apply -f {tempFile}");
    }

    [McpServerTool]
    [Description("Create Kubernetes service")]
    public async Task<string> CreateService(
        [Description("Service name")] string name,
        [Description("Service type")] string type,
        [Description("Port")] int port,
        [Description("Target port")] int targetPort,
        [Description("Selector app label")] string selector,
        [Description("Namespace")] string ns = "default")
    {
        var yaml =
            $$"""
            apiVersion: v1
            kind: Service
            metadata:
              name: {{name}}
              namespace: {{ns}}
            spec:
              type: {{type}}
              selector:
                app: {{selector}}
              ports:
              - port: {{port}}
                targetPort: {{targetPort}}
            """;

        string tempFile = await CreateYamlFile(yaml);

        return await RunKubectlCommand(
            $"apply -f {tempFile}");
    }

    [McpServerTool]
    [Description("Restart deployment rollout")]
    public async Task<string> RestartDeployment(
        [Description("Deployment name")] string deploymentName,
        [Description("Namespace")] string ns = "default")
    {
        return await RunKubectlCommand(
            $"rollout restart deployment " +
            $"{deploymentName} -n {ns}");
    }

    [McpServerTool]
    [Description("Scale deployment replicas")]
    public async Task<string> ScaleDeployment(
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
    public async Task<string> UpdateDeploymentImage(
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

    private static async Task<string> CreateYamlFile(string yaml)
    {
        string yamlDirectory = Path.Combine(Path.GetTempPath(), "yamls");

        Directory.CreateDirectory(yamlDirectory);

        string tempFile = Path.Combine(yamlDirectory,$"{Guid.NewGuid()}.yaml");

        await File.WriteAllTextAsync(
            tempFile,
            yaml);

        return tempFile;
    }
}