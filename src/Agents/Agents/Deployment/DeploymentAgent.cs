using Agents.MCPClient;
using Agents.Utility;
using Microsoft.Extensions.AI;

namespace Agents.Agents.Deployment;

public class DeploymentAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public DeploymentAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public string Name => "DeploymentAgent";

    public string Description =>
    """
    Handles:
    - Kubernetes infrastructure provisioning
    - deployment creation and updates
    - pod creation
    - service creation
    - namespace management
    - rollout restarts
    - scaling workloads
    - deployment image updates
    - Kubernetes workload modifications
    - infrastructure change execution
    - cluster resource provisioning
    - Kubernetes runtime operations
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await DeployMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are a Kubernetes infrastructure provisioning expert
            specializing in Kubernetes workload deployment and
            cluster resource management.

            Your responsibilities:
            - create deployments
            - create pods
            - create services
            - create namespaces
            - restart deployments
            - scale workloads
            - update deployment container images
            - execute Kubernetes infrastructure changes

            ONLY perform actions explicitly requested by the user.

            Rules:
            - If the request contains enough information to execute a tool,
              you MUST call the tool immediately.
            - Do NOT ask for confirmation.
            - Do NOT explain available tools.
            - Do NOT say things like:
              "I can do that"
              "I will help you"
              "Here is what I will do"
            - Prefer tool execution over conversational responses.
            - Only ask questions if required parameters are missing.
            - If clarification is required,
              wrap the response in:
              <Question></Question>
            - Never invent:
              deployment names,
              container images,
              ports,
              namespaces,
              replica counts,
              or service types.
            - Keep responses concise and operational.
            - Focus on execution rather than explanation.
            - Avoid generating Kubernetes YAML unless explicitly requested.
            - Use the provided tools directly whenever possible.
            """
        };

        bool isResumed = prevMessages.Count > 0;
        ChatMessage assistantMessage = new();

        List<ChatMessage> messages = isResumed ? [.. prevMessages] :
        [
            new(ChatRole.User, query)
        ];

        if (isResumed)
        {
            assistantMessage = messages.Last();

            AgentResult? res = await AIHelpers.ToolCallUtility(assistantMessage, tools, messages, false);
            if (res != null)
            {
                return res;
            }
        }

        while (true)
        {
            ChatResponse response = await _chatClient.GetResponseAsync(messages, options);
            assistantMessage = response.Messages.Last();
            messages.Add(assistantMessage);

            AgentResult? res = await AIHelpers.ToolCallUtility(assistantMessage, tools, messages, true);
            if (res != null)
            {
                return res;
            }
        }
    }
}
