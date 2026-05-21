using Microsoft.Extensions.AI;

namespace Agents.Agents.RootReviewer;

public class RootReviewerAgent: IAgent
{
    private readonly IChatClient _chatClient;

    public RootReviewerAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
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
    """;

    public async Task<string> AnalyzeAsync(string request)
    {
        var prompt =
            $$"""
            You are an expert principal Kubernetes Engineer.

            Your role is to:
            - correlate findings
            - identify likely root causes
            - explain reasoning
            - estimate confidence
            - summarize incident

            Agent Findings:
            {{request}}
            """;

        var response =
            await _chatClient
                .GetResponseAsync(prompt);

        return response.Text;
    }
}