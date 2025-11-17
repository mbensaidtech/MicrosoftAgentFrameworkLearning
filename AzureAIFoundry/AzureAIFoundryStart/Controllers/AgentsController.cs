using Microsoft.AspNetCore.Mvc;
using AgentConfiguration;
using AzureAIFoundryStart.Services;
using AzureAIFoundryStart.Models;

namespace AzureAIFoundryStart.Controllers;

/// <summary>
/// Controller for managing agents.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentsController(IAgentService agentService)
    {
        _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
    }

    /// <summary>
    /// Creates a new agent.
    /// </summary>
    /// <param name="request">The request containing the optional agent type.</param>
    /// <returns>Information about the created agent.</returns>
    [HttpPost]
    public async Task<ActionResult<AgentInfo>> CreateAgent([FromBody] CreateAgentRequest? request)
    {
        try
        {
            var agentType = request?.AgentType ?? AgentType.GlobalAgent;
            var agentInfo = await _agentService.CreateAgentAsync(agentType);
            return Ok(agentInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

