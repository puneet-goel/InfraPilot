using Docker.DotNet;
using Docker.DotNet.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace DockerReadMCP.Tools;

[McpServerToolType]
public class DockerReadTools
{
    private readonly DockerClient _client;
    private readonly JsonSerializerOptions _options;

    public DockerReadTools()
    {
        string runtime = Environment.GetEnvironmentVariable("CONTAINER_RUNTIME") ?? "docker";
        string socket =
            Environment.GetEnvironmentVariable(
                "CONTAINER_SOCKET")
            ??
            (
                runtime == "podman"
                    ? RuntimeInformation.IsOSPlatform(
                        OSPlatform.Windows)
                        ? "npipe://./pipe/podman-machine-default"
                        : "unix:///run/user/1000/podman/podman.sock"

                    : RuntimeInformation.IsOSPlatform(
                        OSPlatform.Windows)
                        ? "npipe://./pipe/docker_engine"
                        : "unix:///var/run/docker.sock"
            );

        _client = new DockerClientConfiguration(new Uri(socket))
            .CreateClient();

        _options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    [McpServerTool]
    [Description("List Docker containers")]
    public async Task<string> ListContainers(
        [Description("Include stopped containers")]
        bool all = true)
    {
        try
        {
            IList<ContainerListResponse> containers =
                await _client.Containers.ListContainersAsync(
                    new ContainersListParameters
                    {
                        All = all
                    });

            var summary = containers.Select(c => new
            {
                Id = c.ID[..12],
                Name = c.Names.FirstOrDefault()?.Trim('/'),
                c.Image,
                c.State,
                c.Status,
                c.Created,
                Ports = c.Ports.Select(p => new
                {
                    p.PrivatePort,
                    p.PublicPort,
                    p.Type
                })
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
    [Description("Inspect Docker container")]
    public async Task<string> InspectContainer(
        [Description("Container id or name")]
        string containerId)
    {
        try
        {
            ContainerInspectResponse container =
                await _client.Containers.InspectContainerAsync(
                    containerId);

            var result = new
            {
                Id = container.ID[..12],
                Name = container.Name.Trim('/'),
                container.Config.Image,
                State = new
                {
                    container.State.Status,
                    container.State.Running,
                    container.State.Restarting,
                    container.State.ExitCode,
                    container.State.StartedAt,
                    container.State.FinishedAt
                },
                Network = new
                {
                    container.NetworkSettings.IPAddress,
                    Ports = container.NetworkSettings.Ports.Keys
                },
                Mounts = container.Mounts.Select(m => new
                {
                    m.Source,
                    m.Destination,
                    m.Type,
                    m.RW
                }),
                EnvironmentVariables = container.Config.Env
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
    [Description("Get Docker container logs")]
    public async Task<string> GetContainerLogs(
        [Description("Container id or name")]
        string containerId,

        [Description("Number of log lines")]
        int tail = 100)
    {
        try
        {
            MultiplexedStream stream =
                await _client.Containers.GetContainerLogsAsync(
                    containerId,
                    false,
                    new ContainerLogsParameters
                    {
                        ShowStdout = true,
                        ShowStderr = true,
                        Tail = tail.ToString()
                    });

            MemoryStream memory = new();
            await stream.CopyOutputToAsync(
                Stream.Null,
                memory,
                memory,
                CancellationToken.None);

            return Encoding.UTF8.GetString(
                memory.ToArray());
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
    [Description("List Docker images")]
    public async Task<string> ListImages()
    {
        try
        {
            IList<ImagesListResponse> images =
                await _client.Images.ListImagesAsync(
                    new ImagesListParameters
                    {
                        All = true
                    });

            var summary = images.Select(i => new
            {
                Id = i.ID.Replace("sha256:", "")[..12],
                Tags = i.RepoTags,
                SizeMB = Math.Round(
                    i.Size / 1024d / 1024d,
                    2),
                i.Created
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
    [Description("Inspect Docker image")]
    public async Task<string> InspectImage(
        [Description("Image name or id")]
        string imageName)
    {
        try
        {
            ImageInspectResponse image =
                await _client.Images.InspectImageAsync(
                    imageName);

            var result = new
            {
                Id = image.ID.Replace("sha256:", "")[..12],
                Tags = image.RepoTags,
                SizeMB = Math.Round(
                    image.Size / 1024d / 1024d,
                    2),
                image.Architecture,
                OS = image.Os,
                image.Created,
                EnvironmentVariables = image.Config.Env,
                image.Config.Entrypoint,
                image.Config.Cmd
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
    [Description("List Docker networks")]
    public async Task<string> ListNetworks()
    {
        try
        {
            IList<NetworkResponse> networks =
                await _client.Networks.ListNetworksAsync();

            var summary = networks.Select(n => new
            {
                n.ID,
                n.Name,
                n.Driver,
                n.Scope
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
    [Description("Inspect Docker network")]
    public async Task<string> InspectNetwork(
        [Description("Network id or name")]
        string networkId)
    {
        try
        {
            NetworkResponse network =
                await _client.Networks.InspectNetworkAsync(
                    networkId);

            var result = new
            {
                network.ID,
                network.Name,
                network.Driver,
                network.Scope,
                Containers =
                    network.Containers?.Select(c => new
                    {
                        c.Key,
                        c.Value.Name,
                        c.Value.IPv4Address
                    })
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
    [Description("List Docker volumes")]
    public async Task<string> ListVolumes()
    {
        try
        {
            VolumesListResponse response =
                await _client.Volumes.ListAsync();

            var summary = response.Volumes.Select(v => new
            {
                v.Name,
                v.Driver,
                v.Mountpoint,
                v.CreatedAt
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
    [Description("Get Docker system information")]
    public async Task<string> GetDockerInfo()
    {
        try
        {
            SystemInfoResponse info = await _client.System.GetSystemInfoAsync();

            var result = new
            {
                info.ServerVersion,
                info.OperatingSystem,
                info.OSType,
                info.Architecture,
                info.NCPU,
                info.MemTotal,
                info.Containers,
                info.ContainersRunning,
                info.ContainersStopped,
                info.Images,
                info.Driver,
                info.KernelVersion
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
    [Description("Get Docker version information")]
    public async Task<string> GetDockerVersion()
    {
        try
        {
            VersionResponse version =
                await _client.System.GetVersionAsync();

            return JsonSerializer.Serialize(
                new
                {
                    version.Version,
                    version.APIVersion,
                    version.GoVersion,
                    version.GitCommit,
                    version.Os,
                    version.Arch
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