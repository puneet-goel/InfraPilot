namespace Api.Application.DTO;

public class ReRunWorkflow
{
    public required string WorkflowId { get; set; }
    public required string ExecutionId { get; set; }
    public bool UseSamePlan { get; set; } = true;
}
