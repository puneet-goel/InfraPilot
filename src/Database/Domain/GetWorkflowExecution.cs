namespace Database.Domain;

public class GetWorkflowExecution
{
    public Guid WorkflowId { get; set; }
    public string Status { get; set; } = "";
    public string Reason { get; set; } = "";
    public string CurrentAgent { get; set; } = "";
    public string AgentOutput { get; set; } = "";
}