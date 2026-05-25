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

    public bool IsWriteAgent => true;

    public string Description =>
    """
    Handles:
    - deployment updates
    - YAML generation
    - rollout restarts
    - scaling deployments
    - applying manifests
    - patching workloads
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await DeployMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are a Kubernetes infrastructure provisioning expert who deploy resources.

            Your responsibilities:
            - generate Kubernetes manifests
            - apply infrastructure changes
            - restart deployments
            - scale workloads
            - update deployment images

            ONLY perform actions requested.

            Rules:
            - If the user request contains enough information to execute a tool, you MUST call the tool immediately.
            - Do NOT ask for confirmation.
            - Do NOT explain available tools.
            - Do NOT say "I can do that".
            - Do NOT describe intended actions.
            - Prefer tool execution over conversational responses.
            - Only ask questions if required parameters are missing.
            - If you need to ask question or need any clarification from the user please wrap your message in <Question></Question>
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
