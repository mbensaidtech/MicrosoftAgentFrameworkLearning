using Microsoft.AspNetCore.Mvc;
using AzureOpenAIAgentWithFunctionTools.Models;
using AzureOpenAIAgentWithFunctionTools.Services;

namespace AzureOpenAIAgentWithFunctionTools.Controllers;

/// <summary>
/// Controller for interacting with AI agents.
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
    /// Sends a question to an agent and returns the response.
    /// </summary>
    /// <param name="request">The request containing the question.</param>
    /// <returns>The agent's response.</returns>
    [HttpPost("ask")]
    public async Task<ActionResult<AgentResponse>> Ask([FromBody] AskRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Question is required.");
            }

            var agentResponse = await _agentService.SendMessageAsync(request.Question);
            return Ok(agentResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

