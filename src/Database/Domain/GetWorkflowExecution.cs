namespace Database.Domain;

public class GetWorkflowExecution
{
    public Guid ExecutionId { get; set; }
    public Guid WorkflowId { get; set; }
    public string UserRequest { get; set; } = "";
    public string? WorkflowPlan { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public string? CurrentAgent { get; set; }
    public string? AgentOutput { get; set; }
}