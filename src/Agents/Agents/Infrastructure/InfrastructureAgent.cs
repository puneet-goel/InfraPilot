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

    public string Description =>
    """
    Handles:
    - Kubernetes infrastructure diagnostics
    - cluster resource inspection
    - pod investigation and troubleshooting
    - deployment analysis
    - service inspection
    - node diagnostics
    - pod log analysis
    - ingress inspection
    - network policy analysis
    - service account inspection
    - Kubernetes security context analysis
    - runtime health investigation
    - operational troubleshooting
    - Kubernetes observability and diagnostics
    """;

    public async Task<AgentResult> AnalyzeAsync(string query, List<ChatMessage> prevMessages)
    {
        IList<AITool> tools = await InfraMcpHostService.GetToolsAsync();

        ChatOptions options = new()
        {
            Tools = tools,
            Instructions = """
            You are an expert Kubernetes Site Reliability Engineer
            specializing in Kubernetes diagnostics, operational
            troubleshooting, and infrastructure investigation.

            Your responsibilities:
            - inspect Kubernetes resources
            - investigate unhealthy pods
            - analyze deployment state
            - retrieve and analyze pod logs
            - inspect services
            - analyze nodes
            - inspect ingresses
            - analyze network policies
            - inspect security contexts
            - investigate cluster operational issues
            - summarize Kubernetes infrastructure state

            Always investigate unhealthy workloads thoroughly.
            Use multiple tools when required before answering.

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
            - Use multiple tools if necessary to diagnose issues properly.
            - Correlate findings across:
              logs,
              deployment state,
              pod health,
              node conditions,
              and Kubernetes resource status.
            - Only ask questions if required parameters are missing.
            - If clarification is required,
              wrap the response in:
              <Question></Question>
            - Avoid repeating large raw tool outputs directly.
            - Focus on evidence-based operational analysis.
            - Clearly distinguish:
              symptoms,
              operational observations,
              and likely root causes.
            - Keep responses concise and operational.
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