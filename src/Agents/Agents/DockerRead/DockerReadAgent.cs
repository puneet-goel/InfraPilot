using Agents.MCPClient;
using Agents.Utility;
using Microsoft.Extensions.AI;

namespace Agents.Agents.DockerRead;

public class DockerReadAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public DockerReadAgent(IChatClient chatClient)
    {
        _chatClient = chatClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build(); ;
    }

    public string Name => "DockerReadAgent";

    public bool IsWriteAgent => false;

    public string Description =>
     """
    Handles:
    - Docker container inspection
    - Podman container inspection
    - container diagnostics
    - container log analysis
    - image inspection
    - runtime health investigation
    - Docker and Podman infrastructure analysis
    - network inspection
    - volume inspection
    - runtime system information retrieval
    - operational troubleshooting
    - container runtime observability
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await InfraMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are an expert container runtime SRE engineer specializing
            in Docker and Podman environments.

            Always investigate runtime issues thoroughly.
            Use multiple tools when needed before answering.

            Your responsibilities:
            - inspect containers
            - analyze container logs
            - investigate runtime failures
            - inspect images
            - analyze Docker and Podman networks
            - inspect volumes
            - retrieve runtime system information
            - identify operational issues
            - summarize infrastructure state

            Rules:
            - If the user request contains enough information to execute a tool,
                you MUST call the tool immediately.
            - Do NOT ask for confirmation.
            - Do NOT explain available tools.
            - Do NOT say things like:
                "I can do that"
                "I will help you"
                "Here is what I will do"
            - Prefer tool execution over conversational responses.
            - Use multiple tools if required to diagnose issues properly.
            - Only ask questions if required parameters are missing.
            - If clarification is required,
                wrap your response in:
                <Question></Question>
            - Focus on operational diagnostics and evidence-based analysis.
            - Avoid repeating large raw tool outputs directly.
            - Summarize findings clearly and concisely.
            """
        };

        bool isResumed = prevMessages.Count > 0;
        List<ChatMessage> messages = isResumed ? [.. prevMessages] :
        [
            new(ChatRole.User, query)
        ];

        if (isResumed)
        {
            ChatMessage assistantMessage = messages.Last();

            AgentResult? res = await AIHelpers.ToolCallUtility(assistantMessage, tools, messages);
            if (res != null)
            {
                return res;
            }
        }

        ChatResponse response = await _chatClient.GetResponseAsync(messages, options);
        return new()
        {
            ApprovalRequired = false,
            Messages = [.. response.Messages]
        };
    }
}