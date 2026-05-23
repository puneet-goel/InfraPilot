namespace Database.Domain;

public class GetWorkflow
{
    public Guid Id { get; set; }
    public string UserRequest { get; set; }  = "";
    public string Plan { get; set; } = "";
}