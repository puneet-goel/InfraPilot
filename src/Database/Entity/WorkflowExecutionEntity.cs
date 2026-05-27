using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entity;

[Table("workflow_execution_events", Schema = "infrapilot")]
public class WorkflowExecutionEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("workflow_id")]
    public Guid WorkflowId { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("plan_json", TypeName = "jsonb")]
    public string? Plan { get; set; }

    [Column("current_agent")]
    public string? CurrentAgent { get; set; }

    [Column("agent_output", TypeName = "jsonb")]
    public string? AgentOutput { get; set; }
    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(WorkflowId))]
    public WorkflowEntity Workflow { get; set; } = null!;
}