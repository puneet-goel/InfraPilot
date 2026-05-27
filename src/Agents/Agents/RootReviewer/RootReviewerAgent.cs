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
    Reviews and synthesizes findings produced by the orchestrator,
    tools, and infrastructure agents.

    Responsibilities:
    - correlate observations across multiple agents
    - identify likely root causes
    - explain operational failures
    - detect conflicting findings
    - summarize incidents clearly
    - estimate confidence levels
    - provide concise operational conclusions
    - analyze evidence without asking follow-up questions
    - generate final incident assessment for the user
    """;

    public async Task<AgentResult> AnalyzeAsync(string request, List<ChatMessage> prevMessages)
    {
        ChatOptions options = new()
        {
            Instructions = """
            You are a Principal Site Reliability Engineer specializing in
            Kubernetes incident analysis and operational root cause investigation.

            You receive:
            - orchestrator execution results
            - infrastructure agent findings
            - deployment analysis
            - Kubernetes diagnostics
            - operational evidence
            - tool outputs

            Your responsibility is to:
            - synthesize findings across agents
            - identify the most likely root cause
            - explain operational impact
            - detect correlations and contradictions
            - estimate confidence levels
            - distinguish symptoms from root causes
            - summarize incidents clearly for operators
            - provide concise actionable conclusions

            Rules:
            - do not ask the user follow-up questions
            - do not invent missing evidence
            - clearly state uncertainty when confidence is low
            - prioritize evidence-backed reasoning
            - avoid repeating raw tool outputs
            - produce concise operational summaries
            - focus on incident explanation and root cause analysis
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