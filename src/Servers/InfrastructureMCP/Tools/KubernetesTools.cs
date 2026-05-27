using k8s;
using k8s.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace InfrastructureMCP.Tools;

[McpServerToolType]
public class KubernetesTools
{
    private readonly Kubernetes _client;
    private readonly JsonSerializerOptions _options;

    public KubernetesTools()
    {
        string? host = Environment.GetEnvironmentVariable("KUBE_HOST");
        string? token = Environment.GetEnvironmentVariable("KUBE_TOKEN");
        //string? caCertContent = Environment.GetEnvironmentVariable("KUBE_CA_CERT");

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new Exception("KUBE_HOST environment variable is missing.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new Exception("KUBE_TOKEN environment variable is missing.");
        }

        //if (string.IsNullOrWhiteSpace(caCertContent))
        //{
        //    throw new Exception("KUBE_CA_CERT environment variable is missing.");
        //}

        //byte[] certBytes = Encoding.UTF8.GetBytes(caCertContent);
        //X509Certificate2 caCert = X509Certificate2.CreateFromPem(Encoding.UTF8.GetString(certBytes));

        KubernetesClientConfiguration config = new()
        {
            Host = host,
            AccessToken = token,
            SkipTlsVerify = true,
            //SslCaCerts = new X509Certificate2Collection(caCert)
        };

        _client = new(config);
        _options = new()
        {
            WriteIndented = true
        };
    }

    [McpServerTool] 
    [Description("""
        Get Kubernetes resources.
        
        Valid resource types:
        - pods
        - deployments
        - nodes
        - configmaps
        - secrets
        - svc (services)
    """)]
    public async Task<string> GetResource(
        [Description("Kubernetes resource type")] string resourceType)
    {
        object result = resourceType.ToLower() switch
        {
            "pods" => await _client.CoreV1.ListPodForAllNamespacesAsync(),
            "deployments" => await _client.AppsV1.ListDeploymentForAllNamespacesAsync(),
            "nodes" => await _client.CoreV1.ListNodeAsync(),
            "services" or "svc" => await _client.CoreV1.ListServiceForAllNamespacesAsync(),
            "configmaps" => await _client.CoreV1.ListConfigMapForAllNamespacesAsync(),
            "secrets" => await _client.CoreV1.ListSecretForAllNamespacesAsync(),
            _ => throw new Exception($"Unsupported resource type: {resourceType}")
        };

        return JsonSerializer.Serialize(result, _options);
    }

    [McpServerTool]
    [Description("""
        Get Kubernetes resources.
        
        Valid resource types:
        - pods
        - deployments
        - nodes
        - configmaps
        - secrets
        - svc (services)
    """)]
    public async Task<string> DescribeSpecificResource(
        [Description("Kubernetes resource type name")] string resourceType,
        [Description("Kubernetes resource name")] string resourceName)
    {
        object result = resourceType.ToLower() switch
        {
            "pods" => await _client.CoreV1.ReadNamespacedPodAsync(resourceName, "default"),
            "deployments" => await _client.AppsV1.ReadNamespacedDeploymentAsync(resourceName, "default"),
            "service" or "svc" => await _client.CoreV1.ReadNamespacedServiceAsync(resourceName, "default"),
            "configmaps" =>await _client.CoreV1.ReadNamespacedConfigMapAsync(resourceName, "default"),
            "secrets" => await _client.CoreV1.ReadNamespacedSecretAsync(resourceName, "default"),
            "nodes" => await _client.CoreV1.ReadNodeAsync(resourceName),
            _ => throw new Exception($"Unsupported resource type: {resourceType}")
        };

        return JsonSerializer.Serialize(result, _options);
    }

    [McpServerTool]
    [Description("Get logs for a Kubernetes pod")]
    public async Task<string> GetPodLogs(string podName)
    {
        using Stream logStream = await _client.CoreV1.ReadNamespacedPodLogAsync(podName, "default");
        using StreamReader reader = new(logStream);
        return await reader.ReadToEndAsync();
    }

    [McpServerTool]
    [Description("Get Kubernetes service accounts")]
    public async Task<string> GetServiceAccounts()
    {
        V1ServiceAccountList result = await _client.CoreV1.ListServiceAccountForAllNamespacesAsync();
        return JsonSerializer.Serialize(result, _options);
    }

    [McpServerTool]
    [Description("Get pod security context configuration")]
    public async Task<string> GetPodSecurityContext(
        [Description("Pod name")] string podName)
    {
        V1Pod pod = await _client.CoreV1.ReadNamespacedPodAsync(podName, "default");

        var securityContext = new
        {
            PodSecurityContext = pod.Spec.SecurityContext,
            Containers = pod.Spec.Containers.Select(c => new
            {
                c.Name,
                c.SecurityContext
            })
        };

        return JsonSerializer.Serialize(securityContext, _options);
    }

    [McpServerTool]
    [Description("Get deployment security configuration")]
    public async Task<string> GetDeploymentSecuritySpec(
        [Description("Deployment name")] string deploymentName)
    {
        V1Deployment deployment =await _client.AppsV1.ReadNamespacedDeploymentAsync(deploymentName, "default");

        var securitySpec = new
        {
            PodSecurityContext = deployment.Spec.Template.Spec.SecurityContext,

            Containers = deployment.Spec.Template.Spec.Containers.Select(c => new
            {
                c.Name,
                c.SecurityContext
            })
        };

        return JsonSerializer.Serialize(securitySpec, _options);
    }

    [McpServerTool]
    [Description("Describe Kubernetes service account")]
    public async Task<string> DescribeServiceAccount(
        [Description("Service account name")] string serviceAccount)
    {
        V1ServiceAccount result = await _client.CoreV1.ReadNamespacedServiceAccountAsync(serviceAccount, "default");

        return JsonSerializer.Serialize(result, _options);
    }

    [McpServerTool]
    [Description("Get Kubernetes ingresses")]
    public async Task<string> GetIngresses()
    {
        V1IngressList result = await _client.NetworkingV1.ListIngressForAllNamespacesAsync();

        return JsonSerializer.Serialize(result, _options);
    }

    [McpServerTool]
    [Description("Get Kubernetes network policies")]
    public async Task<string> GetNetworkPolicies()
    {
        V1NetworkPolicyList result = await _client.NetworkingV1.ListNetworkPolicyForAllNamespacesAsync();

        return JsonSerializer.Serialize(result, _options);
    }
}