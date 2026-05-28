using System.Text.Json.Serialization;

namespace Agents.Workflow;

public enum WorkflowEventType
{
    FullMessageHistory
}

public sealed class WorkflowEvent
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WorkflowEventType Type { get; set; }
    public List<AgentOutput> Result { get; set; }
    public bool ApprovalRequired { get; set; } = false;
}