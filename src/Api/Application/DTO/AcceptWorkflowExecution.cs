namespace Api.Application.DTO;

public class AcceptWorkflowExecution
{
    public bool Accept { get; set; } = true;
    public required string ExecutionId { get; set; }
    public string Reason { get; set; } = "";
}
