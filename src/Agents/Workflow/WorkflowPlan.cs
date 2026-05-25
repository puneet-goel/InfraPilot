namespace Agents.Workflow;

public class WorkflowStep
{
    public string AgentName { get; set; } = "";
    public string Task { get; set; } = "";
}

public class WorkflowPlan
{
    public string RuntimeEnvironment { get; set; } = "";
    public List<WorkflowStep> Steps { get; set; } = [];
}

public class ToolCall
{
    public string ToolCallId { get; set; } = "";
    public string ToolName { get; set; } = "";
    public IDictionary<string, object?>? Arguments { get; set; }
    public object? Result { get; set; }
    public Exception? Exception { get; set; }
}

public class AgentChatMessage
{
    public string Role { get; set; } = "";
    public string Text { get; set; } = "";
    public List<ToolCall> ToolCalls { get; set; } = [];
    public bool IsApprovalRequired { get; set; }
    public string ApprovalStatus { get; set; } = ""; // pending, accepted, rejected
    public string ApprovalReason { get; set; } = "";
}

public class AgentOutput
{
    public string AgentName { get; set; } = "";
    public List<AgentChatMessage> Chat { get; set; } = [];
}

public class WorkflowPlanResult
{
    public string RuntimeEnvironment { get; set; } = "";
    public List<AgentOutput> Steps { get; set; } = [];
}