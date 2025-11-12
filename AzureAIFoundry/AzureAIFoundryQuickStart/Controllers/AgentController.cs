using Microsoft.AspNetCore.Mvc;
using AgentConfiguration;
using AzureAIFoundryQuickStart.Services;
using AzureAIFoundryQuickStart.Models;

namespace AzureAIFoundryQuickStart.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentConversationService _agentConversationService;

    public AgentController(IAgentConversationService agentConversationService)
    {
        _agentConversationService = agentConversationService ?? throw new ArgumentNullException(nameof(agentConversationService));
    }

    [HttpPost("message")]
    public async Task<ActionResult<string>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            request.Context ??= new AgentContext();
            request.Context.AgentType = AgentType.GlobalAgent;

            var agentMessage = await _agentConversationService.SendMessageAsync(request);
            return Ok(agentMessage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

