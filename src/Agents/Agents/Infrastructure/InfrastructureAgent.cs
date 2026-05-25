using Agents.MCPClient;
using Agents.Utility;
using Microsoft.Extensions.AI;

namespace Agents.Agents.Infrastructure;

public class InfrastructureAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public InfrastructureAgent(IChatClient chatClient)
    {
        _chatClient = chatClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build(); ;
    }

    public string Name => "InfrastructureAgent";

    public bool IsWriteAgent => false;

    public string Description =>
    """
    Handles:
    - cluster analysis
    - nodes
    - pods
    - deployments
    - logs
    - events
    - configmaps
    - secrets
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await InfraMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are an expert Kubernetes SRE engineer.

            Always investigate unhealthy pods thoroughly.
            Use multiple tools if needed before answering.

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