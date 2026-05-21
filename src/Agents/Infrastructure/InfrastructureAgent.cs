using Microsoft.Extensions.AI;
using Tools.Kubernetes;

namespace Agents.Infrastructure;

public class InfrastructureAgent
{
    private readonly IChatClient _chatClient;
    private readonly KubernetesTools _kubernetesTools;

    public InfrastructureAgent(
        IChatClient chatClient,
        KubernetesTools kubernetesTools)
    {
        _chatClient = chatClient;
        _kubernetesTools = kubernetesTools;
    }

    public async Task<string> AnalyzeClusterAsync()
    {
        ChatOptions options = new()
        {
            Tools =
            [
                AIFunctionFactory.Create(
                    _kubernetesTools.GetPods)
            ]
        };

        var response = await _chatClient.GetResponseAsync(
            """
            Analyze my Kubernetes cluster.
            Use tools if needed.
            """,
            options);

        return response.Text;
    }
}