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

public class WorkflowStepResult
{
    public string AgentName { get; set; } = "";

    public string Output { get; set; } = "";
}

public class WorkflowPlanResult
{
    public string RuntimeEnvironment { get; set; } = "";
    public List<WorkflowStepResult> Steps { get; set; } = [];
}