using Microsoft.Extensions.AI;

namespace Agents.RootReviewer;

public class RootReviewerAgent
{
    private readonly IChatClient _chatClient;

    public RootReviewerAgent(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

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