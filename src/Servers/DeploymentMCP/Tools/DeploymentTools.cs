using k8s;
using k8s.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace DeploymentMCP.Tools;

[McpServerToolType]
public class DeploymentTools
{
    private readonly Kubernetes _client;
    private readonly JsonSerializerOptions _options;

    public DeploymentTools()
    {
        string? host =
            Environment.GetEnvironmentVariable("KUBE_HOST");

        string? token =
            Environment.GetEnvironmentVariable("KUBE_TOKEN");

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new Exception(
                "KUBE_HOST environment variable is missing.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new Exception(
                "KUBE_TOKEN environment variable is missing.");
        }

        KubernetesClientConfiguration config = new()
        {
            Host = host,
            AccessToken = token,
            SkipTlsVerify = true
        };

        _client = new Kubernetes(config);

        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
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
        try
        {
            V1Deployment deployment = new()
            {
                ApiVersion = "apps/v1",
                Kind = "Deployment",
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    NamespaceProperty = ns
                },

                Spec = new V1DeploymentSpec
                {
                    Replicas = replicas,
                    Selector = new V1LabelSelector
                    {
                        MatchLabels = new Dictionary<string, string>
                        {
                            ["app"] = name
                        }
                    },

                    Template = new V1PodTemplateSpec
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                ["app"] = name
                            }
                        },

                        Spec = new V1PodSpec
                        {
                            Containers =
                            [
                                new V1Container
                            {
                                Name = name,
                                Image = image,
                                Ports =
                                [
                                    new V1ContainerPort
                                    {
                                        ContainerPort = port
                                    }
                                ]
                            }
                            ]
                        }
                    }
                }
            };

            V1Deployment result =
                await _client.AppsV1.CreateNamespacedDeploymentAsync(
                    deployment,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Deployment created successfully",
                    result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    result.Spec.Replicas,
                    result.Status?.AvailableReplicas,
                    Image = image
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }

    [McpServerTool]
    [Description("Create Kubernetes namespace")]
    public async Task<string> CreateNamespace(
        [Description("Namespace name")] string name)
    {
        try
        {
            V1Namespace ns = new()
            {
                ApiVersion = "v1",
                Kind = "Namespace",
                Metadata = new V1ObjectMeta
                {
                    Name = name
                }
            };

            V1Namespace result =
                await _client.CoreV1.CreateNamespaceAsync(ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Namespace created successfully",
                    result.Metadata.Name,
                    Status = result.Status?.Phase
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }

    [McpServerTool]
    [Description("Create Kubernetes pod")]
    public async Task<string> CreatePod(
        [Description("Pod name")] string name,

        [Description("Container image")] string image,

        [Description("Container port")] int port,

        [Description("Namespace")] string ns = "default")
    {
        try
        {
            V1Pod pod = new()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    NamespaceProperty = ns
                },

                Spec = new V1PodSpec
                {
                    Containers =
                    [
                        new V1Container
                    {
                        Name = name,
                        Image = image,
                        Ports =
                        [
                            new V1ContainerPort
                            {
                                ContainerPort = port
                            }
                        ]
                    }
                    ]
                }
            };

            V1Pod result =
                await _client.CoreV1.CreateNamespacedPodAsync(
                    pod,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Pod created successfully",
                    result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    result.Status?.Phase,
                    result.Status?.PodIP,
                    Node = result.Spec?.NodeName,
                    Image = image
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
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
        try
        {
            V1Service service = new()
            {
                ApiVersion = "v1",
                Kind = "Service",
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    NamespaceProperty = ns
                },

                Spec = new V1ServiceSpec
                {
                    Type = type,
                    Selector = new Dictionary<string, string>
                    {
                        ["app"] = selector
                    },
                    Ports =
                    [
                        new V1ServicePort
                    {
                        Port = port,
                        TargetPort = targetPort
                    }
                    ]
                }
            };

            V1Service result =
                await _client.CoreV1.CreateNamespacedServiceAsync(
                    service,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Service created successfully",
                    result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    result.Spec.Type,
                    result.Spec.ClusterIP,
                    Ports = result.Spec.Ports.Select(p => new
                    {
                        p.Port,
                        p.TargetPort
                    })
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }

    [McpServerTool]
    [Description("Restart deployment rollout")]
    public async Task<string> RestartDeployment(
        [Description("Deployment name")] string deploymentName,

        [Description("Namespace")] string ns = "default")
    {
        try
        {
            V1Deployment deployment =
                await _client.AppsV1.ReadNamespacedDeploymentAsync(
                    deploymentName,
                    ns);

            deployment.Spec.Template.Metadata ??=
                new V1ObjectMeta();

            deployment.Spec.Template.Metadata.Annotations ??=
                new Dictionary<string, string>();

            deployment.Spec.Template.Metadata.Annotations[
                "kubectl.kubernetes.io/restartedAt"
            ] = DateTime.UtcNow.ToString("O");

            V1Deployment result =
                await _client.AppsV1.ReplaceNamespacedDeploymentAsync(
                    deployment,
                    deploymentName,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Deployment restarted successfully",
                    result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    result.Spec.Replicas,
                    result.Status?.UpdatedReplicas
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }

    [McpServerTool]
    [Description("Scale deployment replicas")]
    public async Task<string> ScaleDeployment(
        [Description("Deployment name")] string deploymentName,

        [Description("Replica count")] int replicas,

        [Description("Namespace")] string ns = "default")
    {
        try
        {
            V1Deployment deployment =
                await _client.AppsV1.ReadNamespacedDeploymentAsync(
                    deploymentName,
                    ns);

            deployment.Spec.Replicas = replicas;

            V1Deployment result =
                await _client.AppsV1.ReplaceNamespacedDeploymentAsync(
                    deployment,
                    deploymentName,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Deployment scaled successfully",
                    result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    DesiredReplicas = result.Spec.Replicas,
                    result.Status?.ReadyReplicas,
                    result.Status?.AvailableReplicas
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }

    [McpServerTool]
    [Description("Patch deployment image")]
    public async Task<string> UpdateDeploymentImage(
        [Description("Deployment name")] string deploymentName,

        [Description("Container name")] string containerName,

        [Description("New image")] string image,

        [Description("Namespace")] string ns = "default")
    {
        try
        {
            V1Deployment deployment =
                await _client.AppsV1.ReadNamespacedDeploymentAsync(
                    deploymentName,
                    ns);

            V1Container? container =
                deployment.Spec.Template.Spec.Containers
                    .FirstOrDefault(c => c.Name == containerName);

            if (container is null)
            {
                throw new Exception(
                    $"Container '{containerName}' not found.");
            }

            container.Image = image;

            V1Deployment result =
                await _client.AppsV1.ReplaceNamespacedDeploymentAsync(
                    deployment,
                    deploymentName,
                    ns);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Deployment image updated successfully",
                    Deployment = result.Metadata.Name,
                    Namespace = result.Metadata.NamespaceProperty,
                    Container = containerName,
                    NewImage = image,
                    result.Status?.UpdatedReplicas
                },
                _options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(
                new
                {
                    Error = ex.Message
                },
                _options);
        }
    }
}