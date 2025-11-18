using Microsoft.AspNetCore.Mvc;
using AgentConfiguration;
using AzureAIFoundrySO.Services;
using AzureAIFoundrySO.Models;

namespace AzureAIFoundrySO.Controllers;

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
    /// Creates a new BookRecommendationAgent.
    /// </summary>
    /// <returns>Information about the created agent.</returns>
    [HttpPost]
    public async Task<ActionResult<AgentInfo>> CreateAgent()
    {
        try
        {
            var agentInfo = await _agentService.CreateAgentAsync(AgentType.BookRecommendationAgent);
            return Ok(agentInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

