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
    [Description("Get Kubernetes resources summary")]
    public async Task<string> GetResource(
        [Description("Kubernetes resource type")]
        string resourceType)
    {
        try
        {
            object result = resourceType.ToLower() switch
            {
                "pods" => await GetPodsSummary(),
                "deployments" => await GetDeploymentsSummary(),
                "nodes" => await GetNodesSummary(),
                "services" or "svc" => await GetServicesSummary(),
                "configmaps" => await GetConfigMapsSummary(),
                "secrets" => await GetSecretsSummary(),
                _ => throw new Exception(
                    $"Unsupported resource type: {resourceType}")
            };

            return JsonSerializer.Serialize(result, _options);
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
    [Description("Describe specific Kubernetes resource")]
    public async Task<string> DescribeSpecificResource(
        [Description("Kubernetes resource type")]
        string resourceType,

        [Description("Kubernetes resource name")]
        string resourceName,

        [Description("Namespace")]
        string ns = "default")
    {
        try
        {
            object result = resourceType.ToLower() switch
            {
                "pods" or "pod" =>
                    await DescribePod(resourceName, ns),

                "deployments" or "deployment" =>
                    await DescribeDeployment(resourceName, ns),

                "services" or "service" or "svc" =>
                    await DescribeService(resourceName, ns),

                "configmaps" or "configmap" =>
                    await DescribeConfigMap(resourceName, ns),

                "secrets" or "secret" =>
                    await DescribeSecret(resourceName, ns),

                "nodes" or "node" =>
                    await DescribeNode(resourceName),

                _ => throw new Exception(
                    $"Unsupported resource type: {resourceType}")
            };

            return JsonSerializer.Serialize(result, _options);
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
    [Description("Get logs for a Kubernetes pod")]
    public async Task<string> GetPodLogs(
        [Description("Pod name")]
        string podName,

        [Description("Namespace")]
        string ns = "default")
    {
        try
        {
            using Stream logStream =
                await _client.CoreV1.ReadNamespacedPodLogAsync(
                    podName,
                    ns);

            using StreamReader reader = new(logStream);

            string logs = await reader.ReadToEndAsync();

            return string.Join(
                Environment.NewLine,
                logs.Split('\n').TakeLast(100));
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
    [Description("Get Kubernetes service accounts")]
    public async Task<string> GetServiceAccounts()
    {
        try
        {
            V1ServiceAccountList result =
                await _client.CoreV1.ListServiceAccountForAllNamespacesAsync();

            var summary = result.Items.Select(sa => new
            {
                sa.Metadata.Name,
                Namespace = sa.Metadata.NamespaceProperty,
                Secrets = sa.Secrets?.Count ?? 0
            });

            return JsonSerializer.Serialize(summary, _options);
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
    [Description("Get pod security context configuration")]
    public async Task<string> GetPodSecurityContext(
        [Description("Pod name")]
        string podName,

        [Description("Namespace")]
        string ns = "default")
    {
        try
        {
            V1Pod pod =
                await _client.CoreV1.ReadNamespacedPodAsync(
                    podName,
                    ns);

            var securityContext = new
            {
                Pod = pod.Metadata.Name,
                Namespace = pod.Metadata.NamespaceProperty,
                PodSecurityContext = pod.Spec.SecurityContext,
                Containers = pod.Spec.Containers.Select(c => new
                {
                    c.Name,
                    c.SecurityContext
                })
            };

            return JsonSerializer.Serialize(securityContext, _options);
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
    [Description("Get deployment security configuration")]
    public async Task<string> GetDeploymentSecuritySpec(
        [Description("Deployment name")]
        string deploymentName,

        [Description("Namespace")]
        string ns = "default")
    {
        try
        {
            V1Deployment deployment =
                await _client.AppsV1.ReadNamespacedDeploymentAsync(
                    deploymentName,
                    ns);

            var securitySpec = new
            {
                Deployment = deployment.Metadata.Name,
                Namespace = deployment.Metadata.NamespaceProperty,
                PodSecurityContext =
                    deployment.Spec.Template.Spec.SecurityContext,
                Containers =
                    deployment.Spec.Template.Spec.Containers.Select(c => new
                    {
                        c.Name,
                        c.SecurityContext
                    })
            };

            return JsonSerializer.Serialize(securitySpec, _options);
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
    [Description("Describe Kubernetes service account")]
    public async Task<string> DescribeServiceAccount(
        [Description("Service account name")]
        string serviceAccount,

        [Description("Namespace")]
        string ns = "default")
    {
        try
        {
            V1ServiceAccount result =
                await _client.CoreV1.ReadNamespacedServiceAccountAsync(
                    serviceAccount,
                    ns);

            var summary = new
            {
                result.Metadata.Name,
                Namespace = result.Metadata.NamespaceProperty,
                Secrets = result.Secrets?.Select(s => s.Name)
            };

            return JsonSerializer.Serialize(summary, _options);
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
    [Description("Get Kubernetes ingresses")]
    public async Task<string> GetIngresses()
    {
        try
        {
            V1IngressList result =
                await _client.NetworkingV1.ListIngressForAllNamespacesAsync();

            var summary = result.Items.Select(i => new
            {
                i.Metadata.Name,
                Namespace = i.Metadata.NamespaceProperty,
                Hosts = i.Spec.Rules?.Select(r => r.Host)
            });

            return JsonSerializer.Serialize(summary, _options);
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
    [Description("Get Kubernetes network policies")]
    public async Task<string> GetNetworkPolicies()
    {
        try
        {
            V1NetworkPolicyList result =
                await _client.NetworkingV1.ListNetworkPolicyForAllNamespacesAsync();

            var summary = result.Items.Select(np => new
            {
                np.Metadata.Name,
                Namespace = np.Metadata.NamespaceProperty,
                np.Spec.PolicyTypes
            });

            return JsonSerializer.Serialize(summary, _options);
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

    private async Task<object> GetPodsSummary()
    {
        V1PodList pods =
            await _client.CoreV1.ListPodForAllNamespacesAsync();

        return pods.Items.Select(p => new
        {
            p.Metadata.Name,
            Namespace = p.Metadata.NamespaceProperty,
            p.Status.Phase,
            Node = p.Spec.NodeName,
            p.Status.PodIP,
            Restarts = p.Status.ContainerStatuses?
                .Sum(c => c.RestartCount),
            Containers = p.Spec.Containers.Select(c => new
            {
                c.Name,
                c.Image
            })
        });
    }

    private async Task<object> GetDeploymentsSummary()
    {
        V1DeploymentList deployments =
            await _client.AppsV1.ListDeploymentForAllNamespacesAsync();

        return deployments.Items.Select(d => new
        {
            d.Metadata.Name,
            Namespace = d.Metadata.NamespaceProperty,
            d.Spec.Replicas,
            d.Status.AvailableReplicas,
            d.Status.ReadyReplicas,
            d.Status.UpdatedReplicas,
            Images = d.Spec.Template.Spec.Containers
                .Select(c => c.Image)
        });
    }

    private async Task<object> GetNodesSummary()
    {
        V1NodeList nodes =
            await _client.CoreV1.ListNodeAsync();

        return nodes.Items.Select(n => new
        {
            n.Metadata.Name,

            n.Status.NodeInfo.KubeletVersion,
            OS = n.Status.NodeInfo.OperatingSystem,

            n.Status.NodeInfo.Architecture,
            Conditions = n.Status.Conditions
                .Select(c => new
                {
                    c.Type,
                    c.Status
                })
        });
    }

    private async Task<object> GetServicesSummary()
    {
        V1ServiceList services =
            await _client.CoreV1.ListServiceForAllNamespacesAsync();

        return services.Items.Select(s => new
        {
            s.Metadata.Name,
            Namespace = s.Metadata.NamespaceProperty,
            s.Spec.Type,
            s.Spec.ClusterIP,
            Ports = s.Spec.Ports.Select(p => new
            {
                p.Port,
                p.TargetPort
            })
        });
    }

    private async Task<object> GetConfigMapsSummary()
    {
        V1ConfigMapList configMaps =
            await _client.CoreV1.ListConfigMapForAllNamespacesAsync();

        return configMaps.Items.Select(c => new
        {
            c.Metadata.Name,
            Namespace = c.Metadata.NamespaceProperty,
            c.Data?.Keys
        });
    }

    private async Task<object> GetSecretsSummary()
    {
        V1SecretList secrets =
            await _client.CoreV1.ListSecretForAllNamespacesAsync();

        return secrets.Items.Select(s => new
        {
            s.Metadata.Name,
            Namespace = s.Metadata.NamespaceProperty,
            s.Type,
            s.Data?.Keys
        });
    }

    private async Task<object> DescribePod(
        string name,
        string ns)
    {
        V1Pod pod =
            await _client.CoreV1.ReadNamespacedPodAsync(
                name,
                ns);

        return new
        {
            pod.Metadata.Name,
            Namespace = pod.Metadata.NamespaceProperty,
            pod.Status.Phase,
            Node = pod.Spec.NodeName,
            pod.Status.PodIP,
            pod.Status.StartTime,
            RestartCount = pod.Status.ContainerStatuses?
                .Sum(c => c.RestartCount),
            Containers = pod.Spec.Containers.Select(c => new
            {
                c.Name,
                c.Image,
                Ports = c.Ports?.Select(p => p.ContainerPort)
            }),
            Conditions = pod.Status.Conditions?.Select(c => new
            {
                c.Type,
                c.Status,
                c.Reason
            })
        };
    }

    private async Task<object> DescribeDeployment(
        string name,
        string ns)
    {
        V1Deployment deployment =
            await _client.AppsV1.ReadNamespacedDeploymentAsync(
                name,
                ns);

        return new
        {
            deployment.Metadata.Name,
            Namespace = deployment.Metadata.NamespaceProperty,
            deployment.Spec.Replicas,
            deployment.Status.ReadyReplicas,
            deployment.Status.AvailableReplicas,
            deployment.Status.UpdatedReplicas,
            Strategy = deployment.Spec.Strategy.Type,
            Containers = deployment.Spec.Template.Spec.Containers
                .Select(c => new
                {
                    c.Name,
                    c.Image
                })
        };
    }

    private async Task<object> DescribeService(
        string name,
        string ns)
    {
        V1Service service =
            await _client.CoreV1.ReadNamespacedServiceAsync(
                name,
                ns);

        return new
        {
            service.Metadata.Name,
            Namespace = service.Metadata.NamespaceProperty,
            service.Spec.Type,
            service.Spec.ClusterIP,
            service.Spec.Selector,
            Ports = service.Spec.Ports.Select(p => new
            {
                p.Name,
                p.Port,
                p.TargetPort,
                p.Protocol
            })
        };
    }

    private async Task<object> DescribeConfigMap(
        string name,
        string ns)
    {
        V1ConfigMap configMap =
            await _client.CoreV1.ReadNamespacedConfigMapAsync(
                name,
                ns);

        return new
        {
            configMap.Metadata.Name,
            Namespace = configMap.Metadata.NamespaceProperty,
            configMap.Data?.Keys
        };
    }

    private async Task<object> DescribeSecret(
        string name,
        string ns)
    {
        V1Secret secret =
            await _client.CoreV1.ReadNamespacedSecretAsync(
                name,
                ns);

        return new
        {
            secret.Metadata.Name,
            Namespace = secret.Metadata.NamespaceProperty,
            secret.Type,
            secret.Data?.Keys
        };
    }

    private async Task<object> DescribeNode(string name)
    {
        V1Node node =
            await _client.CoreV1.ReadNodeAsync(name);

        return new
        {
            node.Metadata.Name,
            node.Status.NodeInfo.KubeletVersion,
            OS = node.Status.NodeInfo.OperatingSystem,
            node.Status.NodeInfo.Architecture,
            node.Status.NodeInfo.KernelVersion,
            ContainerRuntime =
                node.Status.NodeInfo.ContainerRuntimeVersion,
            Conditions = node.Status.Conditions.Select(c => new
            {
                c.Type,
                c.Status,
                c.Reason
            })
        };
    }
}
