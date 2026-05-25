using Api.Application.DTO;
using Api.Application.Interface;
using Database.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("workflowExecution")]
public class WorkflowExecutionController : ControllerBase
{
    private readonly IWorkflowExecutionService _workflowExecutionService;

    public WorkflowExecutionController(IWorkflowExecutionService workflowExecutionService)
    {
        _workflowExecutionService = workflowExecutionService;
    }

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

    [HttpGet("acceptWorkflowExecution")]
    public async Task<IActionResult> AcceptWorkflowExecution([FromBody] AcceptWorkflowExecution req)
    {
        await _workflowExecutionService.AcceptWorkflowExecution(req);
        return Ok(new object());
    }
}