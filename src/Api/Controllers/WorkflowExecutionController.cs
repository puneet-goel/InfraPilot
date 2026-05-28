using Agents.EventBus;
using Agents.Workflow;
using Api.Application.DTO;
using Api.Application.Interface;
using Database.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Channels;

namespace Api.Controllers;

[ApiController]
[Route("workflowExecution")]
public class WorkflowExecutionController(IWorkflowExecutionService workflowExecutionService, WorkflowEventBus eventBus) : ControllerBase
{
    private readonly IWorkflowExecutionService _workflowExecutionService = workflowExecutionService;
    private readonly WorkflowEventBus _eventBus = eventBus;

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        GetWorkflowExecution? workflow = await _workflowExecutionService.GetWorkflowExecutionStatusAsync(id);
        return Ok(workflow);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        List<GetWorkflowExecution> workflows = await _workflowExecutionService.GetAllWorkflowExecutionStatusAsync();
        return Ok(workflows);
    }

    [HttpPost("acceptWorkflowExecution")]
    public async Task<IActionResult> AcceptWorkflowExecution([FromBody] AcceptWorkflowExecution req)
    {
        await _workflowExecutionService.AcceptWorkflowExecution(req);
        return Ok(new object());
    }

    [HttpGet("{executionId}/workflowEvents")]
    public async Task WorkflowEvents(string executionId)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        (Guid subscriptionId, ChannelReader<WorkflowEvent> reader) =
            _eventBus.Subscribe(Guid.Parse(executionId));

        try
        {
            await foreach (WorkflowEvent evt in reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                string json = JsonSerializer.Serialize(evt);

                await Response.WriteAsync($"data: {json}\n\n");

                await Response.Body
                    .FlushAsync();
            }
        }
        finally
        {
            _eventBus.Unsubscribe(Guid.Parse(executionId), subscriptionId);
        }
    }
}