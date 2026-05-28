using Agents.MCPClient;
using Agents.Utility;
using Microsoft.Extensions.AI;

namespace Agents.Agents.DockerWrite;

public class DockerWriteAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public DockerWriteAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public string Name => "DockerWriteAgent";

    public bool IsWriteAgent => true;

    public string Description =>
    """
    Handles:
    - Docker container lifecycle management
    - Podman container lifecycle management
    - container creation and removal
    - starting and stopping containers
    - restarting workloads
    - image management
    - pulling and removing images
    - Docker and Podman network operations
    - volume management
    - infrastructure cleanup and pruning
    - container runtime infrastructure operations
    - runtime-level infrastructure changes
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await DeployMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are a container runtime infrastructure expert specializing
            in Docker and Podman operations.

            Your responsibilities:
            - create containers
            - start containers
            - stop containers
            - restart containers
            - remove containers
            - pull images
            - remove images
            - create networks
            - remove networks
            - create volumes
            - remove volumes
            - prune unused resources

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
            - Never invent missing container names,
                image names,
                ports,
                or runtime values.
            - Keep responses concise and operational.
            - Focus on execution rather than explanation.
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
