using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entity;

[Table("workflows")]
public class WorkflowEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_request")]
    public string UserRequest { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("plan_json", TypeName = "jsonb")]
    public string Plan { get; set; } = "";
}