namespace Agents.Workflow;

public class WorkflowStep
{
    public string AgentName { get; set; } = "";

    public string Task { get; set; } = "";
}

public class WorkflowPlan
{
    public List<WorkflowStep> Steps { get; set; } = [];
}

public class WorkflowStepResult
{
    public string AgentName { get; set; } = "";

    public string Output { get; set; } = "";
}