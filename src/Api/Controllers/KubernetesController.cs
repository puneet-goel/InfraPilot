using Agents.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class KubernetesController : ControllerBase
{
    private readonly InfrastructureAgent _infrastructureAgent;

    public KubernetesController(
        InfrastructureAgent infrastructureAgent)
    {
        _infrastructureAgent = infrastructureAgent;
    }

    [HttpGet("analyze")]
    public async Task<IActionResult> Analyze()
    {
        var result = await _infrastructureAgent
            .AnalyzeClusterAsync();

        return Ok(result);
    }
}