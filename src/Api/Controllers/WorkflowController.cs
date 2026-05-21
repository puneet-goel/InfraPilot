using Agents.Agents.Orchestrator;
using Agents.Workflow;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("workflow")]
public class WorkflowController : ControllerBase
{
    private readonly OrchestratorAgent _orchestratorAgent;
    private readonly WorkflowEngine _workflowEngine;

    public WorkflowController(OrchestratorAgent orchestratorAgent, WorkflowEngine workflowClient)
    {
        _orchestratorAgent = orchestratorAgent;
        _workflowEngine = workflowClient;
    }

    [HttpPost("investigate")]
    public async Task<IActionResult> Investigate([FromBody] string request)
    {
        WorkflowPlan plan = await _orchestratorAgent.CreatePlanAsync(request);
        string result = await _workflowEngine.ExecuteAsync(plan);
        return Ok(result);
    }
}