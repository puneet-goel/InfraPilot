using Api.Application.DTO;
using Api.Application.Interface;
using Database.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("workflow")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    public WorkflowController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] string request)
    {
        CreateWorkflow workflow = await _workflowService.CreateWorkflowAsync(request);
        return Ok(workflow);
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        GetWorkflow? workflow = await _workflowService.GetWorkflowAsync(id);
        return Ok(workflow);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        List<GetWorkflow> workflows = await _workflowService.GetAllWorkflowAsync();
        return Ok(workflows);
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunWorkflow([FromBody] ReRunWorkflow req)
    {
        await _workflowService.RunWorkflow(req);
        return Ok(new object());
    }
}