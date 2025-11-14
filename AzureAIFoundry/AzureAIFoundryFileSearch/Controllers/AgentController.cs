using Microsoft.AspNetCore.Mvc;
using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared.Models;
using AzureAIFoundryFileSearch.Services;
using AzureAIFoundryFileSearch.Models;
using System.Linq;

namespace AzureAIFoundryFileSearch.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentConversationService _agentConversationService;
    private readonly AgentConfig _agentConfig;

    public AgentController(IAgentConversationService agentConversationService, AgentConfig agentConfig)
    {
        _agentConversationService = agentConversationService ?? throw new ArgumentNullException(nameof(agentConversationService));
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));
    }

    [HttpPost("message")]
    public async Task<ActionResult<AgentResponse>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            request.Context ??= new AgentContext();
            request.Context.AgentType = AgentType.ProductSearchAgent;

            var agentMessage = await _agentConversationService.SendMessageAsync(request);
            return Ok(agentMessage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("init-all-vector-stores")]
    public async Task<ActionResult<InitVectorStoresResponse>> InitAllVectorStores()
    {
        try
        {
            var initializedStores = await _agentConversationService.InitAllVectorStoresAsync();
            return Ok(new InitVectorStoresResponse
            {
                InitializedStores = initializedStores
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

