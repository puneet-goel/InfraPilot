namespace Api.Application.DTO;

public class AcceptWorkflowExecution
{
    public bool Accept { get; set; }
    public string ExecutionId { get; set; } = "";
    public string Reason { get; set; } = "";
}
