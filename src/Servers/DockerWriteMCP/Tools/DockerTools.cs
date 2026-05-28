using Docker.DotNet;
using Docker.DotNet.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DockerWriteMCP.Tools;

[McpServerToolType]
public class DockerWriteTools
{
    private readonly DockerClient _client;
    private readonly JsonSerializerOptions _options;

    public DockerWriteTools()
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
    [Description("Start Docker container")]
    public async Task<string> StartContainer(
        [Description("Container id or name")]
        string containerId)
    {
        try
        {
            bool started =
                await _client.Containers.StartContainerAsync(
                    containerId,
                    new ContainerStartParameters());

            return JsonSerializer.Serialize(
                new
                {
                    Message = started
                        ? "Container started successfully"
                        : "Container failed to start",

                    Container = containerId
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
    [Description("Stop Docker container")]
    public async Task<string> StopContainer(
        [Description("Container id or name")]
        string containerId)
    {
        try
        {
            bool stopped =
                await _client.Containers.StopContainerAsync(
                    containerId,
                    new ContainerStopParameters());

            return JsonSerializer.Serialize(
                new
                {
                    Message = stopped
                        ? "Container stopped successfully"
                        : "Container failed to stop",

                    Container = containerId
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
    [Description("Restart Docker container")]
    public async Task<string> RestartContainer(
        [Description("Container id or name")]
        string containerId)
    {
        try
        {
            await _client.Containers.RestartContainerAsync(
                containerId,
                new ContainerRestartParameters());

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Container restarted successfully",
                    Container = containerId
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
    [Description("Remove Docker container")]
    public async Task<string> RemoveContainer(
        [Description("Container id or name")]
        string containerId,

        [Description("Force remove container")]
        bool force = false)
    {
        try
        {
            await _client.Containers.RemoveContainerAsync(
                containerId,
                new ContainerRemoveParameters
                {
                    Force = force
                });

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Container removed successfully",
                    Container = containerId,
                    Force = force
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
    [Description("Pull Docker image")]
    public async Task<string> PullImage(
        [Description("Image name")]
        string image)
    {
        try
        {
            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = image
                },
                null,
                new Progress<JSONMessage>());

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Image pulled successfully",
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
    [Description("Remove Docker image")]
    public async Task<string> RemoveImage(
        [Description("Image name or id")]
        string image,

        [Description("Force remove image")]
        bool force = false)
    {
        try
        {
            await _client.Images.DeleteImageAsync(
                image,
                new ImageDeleteParameters
                {
                    Force = force
                });

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Image removed successfully",
                    Image = image,
                    Force = force
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
    [Description("Create Docker container")]
    public async Task<string> CreateContainer(
        [Description("Container name")]
        string name,

        [Description("Docker image")]
        string image,

        [Description("Host port")]
        string hostPort,

        [Description("Container port")]
        string containerPort)
    {
        try
        {
            CreateContainerResponse response =
                await _client.Containers.CreateContainerAsync(
                    new CreateContainerParameters
                    {
                        Name = name,
                        Image = image,
                        ExposedPorts =
                            new Dictionary<string, EmptyStruct>
                            {
                                [$"{containerPort}/tcp"] = default
                            },

                        HostConfig = new HostConfig
                        {
                            PortBindings =
                                new Dictionary<string, IList<PortBinding>>
                                {
                                    [$"{containerPort}/tcp"] =
                                    [
                                        new PortBinding
                                    {
                                        HostPort = hostPort
                                    }
                                    ]
                                }
                        }
                    });

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Container created successfully",
                    response.ID,
                    Name = name,
                    Image = image,
                    PortMapping =
                        $"{hostPort}:{containerPort}"
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
    [Description("Create Docker network")]
    public async Task<string> CreateNetwork(
        [Description("Network name")]
        string networkName,

        [Description("Network driver")]
        string driver = "bridge")
    {
        try
        {
            NetworksCreateResponse response =
                await _client.Networks.CreateNetworkAsync(
                    new NetworksCreateParameters
                    {
                        Name = networkName,
                        Driver = driver
                    });

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Network created successfully",
                    response.ID,
                    Network = networkName,
                    Driver = driver
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
    [Description("Remove Docker network")]
    public async Task<string> RemoveNetwork(
        [Description("Network id or name")]
        string networkId)
    {
        try
        {
            await _client.Networks.DeleteNetworkAsync(
                networkId);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Network removed successfully",
                    Network = networkId
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
    [Description("Create Docker volume")]
    public async Task<string> CreateVolume(
        [Description("Volume name")]
        string volumeName)
    {
        try
        {
            VolumeResponse response =
                await _client.Volumes.CreateAsync(
                    new VolumesCreateParameters
                    {
                        Name = volumeName
                    });

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Volume created successfully",
                    response.Name,
                    response.Driver,
                    response.Mountpoint
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
    [Description("Remove Docker volume")]
    public async Task<string> RemoveVolume(
        [Description("Volume name")]
        string volumeName,

        [Description("Force remove volume")]
        bool force = false)
    {
        try
        {
            await _client.Volumes.RemoveAsync(
                volumeName,
                force);

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Volume removed successfully",
                    Volume = volumeName,
                    Force = force
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
    [Description("Prune unused Docker containers")]
    public async Task<string> PruneContainers()
    {
        try
        {
            ContainersPruneResponse response =
                await _client.Containers.PruneContainersAsync(
                    new ContainersPruneParameters());

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Unused containers pruned",
                    DeletedContainers =
                        response.ContainersDeleted,
                    response.SpaceReclaimed
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
    [Description("Prune unused Docker images")]
    public async Task<string> PruneImages()
    {
        try
        {
            ImagesPruneResponse response =
                await _client.Images.PruneImagesAsync(
                    new ImagesPruneParameters());

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Unused images pruned",
                    DeletedImages =
                        response.ImagesDeleted,
                    response.SpaceReclaimed
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
    [Description("Prune unused Docker volumes")]
    public async Task<string> PruneVolumes()
    {
        try
        {
            VolumesPruneResponse response =
                await _client.Volumes.PruneAsync();

            return JsonSerializer.Serialize(
                new
                {
                    Message = "Unused volumes pruned",
                    DeletedVolumes =
                        response.VolumesDeleted,
                    response.SpaceReclaimed
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