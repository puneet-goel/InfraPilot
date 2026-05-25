using Microsoft.Extensions.AI;

namespace Agents.Agents.RootReviewer;

public class RootReviewerAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public RootReviewerAgent(IChatClient chatClient)
    {
        _chatClient = chatClient
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
    }

    public string Name => "RootReviewerAgent";

    public bool IsWriteAgent => false;

    public string Description => """
    Handles:
    - root cause analysis
    - incident investigation
    - correlating findings from multiple agents
    - identifying likely causes of failures
    - summarizing operational issues
    - confidence-based reasoning
    - incident review and explanation
    - you dont ask questions from user you just analyse the query/results
    """;

    public async Task<AgentResult> AnalyzeAsync(string request, List<ChatMessage> prevMessages)
    {
        ChatOptions options = new()
        {
            Instructions = """
            You are an expert principal Kubernetes Engineer.

            Your role is to:
            - correlate findings
            - identify likely root causes
            - explain reasoning
            - estimate confidence
            - summarize incident
            - you dont ask questions from user you just analyse the query/results
            """
        };

        ChatResponse response =
            await _chatClient
                .GetResponseAsync(request, options);

        return new()
        {
            ApprovalRequired = false,
            Messages = [.. response.Messages]
        };
    }
}