using Microsoft.AspNetCore.Mvc;
using AgentConfiguration;
using AgentConfig = AgentConfiguration.AgentConfiguration;
using AzureAIFoundryShared.Models;
using AzureAIFoundryStart.Services;
using AzureAIFoundryStart.Models;

namespace AzureAIFoundryStart.Controllers;

/// <summary>
/// Controller for managing threads (conversations) and messages.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ThreadsController : ControllerBase
{
    private readonly IThreadService _threadService;
    private readonly AgentConfig _agentConfig;

    public ThreadsController(IThreadService threadService, AgentConfig agentConfig)
    {
        _threadService = threadService ?? throw new ArgumentNullException(nameof(threadService));
        _agentConfig = agentConfig ?? throw new ArgumentNullException(nameof(agentConfig));
    }

    /// <summary>
    /// Creates a new conversation (thread) for an agent and sends the first message.
    /// </summary>
    /// <param name="request">The request containing the agent ID and message.</param>
    /// <returns>The agent's response.</returns>
    [HttpPost]
    public async Task<ActionResult<AgentResponse>> CreateThread([FromBody] CreateThreadRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AgentId))
            {
                return BadRequest("AgentId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message is required.");
            }

            var sendMessageRequest = new SendMessageRequest
            {
                Message = request.Message,
                Context = new AgentContext
                {
                    AgentId = request.AgentId
                }
            };

            var agentMessage = await _threadService.SendMessageAsync(sendMessageRequest);
            return Ok(agentMessage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a message to an existing conversation (thread).
    /// </summary>
    /// <param name="threadId">The ID of the existing thread (conversation).</param>
    /// <param name="request">The request containing the agent ID and message.</param>
    /// <returns>The agent's response.</returns>
    [HttpPost("{threadId}/messages")]
    public async Task<ActionResult<AgentResponse>> AddMessage(string threadId, [FromBody] AddMessageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AgentId))
            {
                return BadRequest("AgentId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message is required.");
            }

            var sendMessageRequest = new SendMessageRequest
            {
                Message = request.Message,
                Context = new AgentContext
                {
                    AgentId = request.AgentId,
                    ThreadId = threadId
                }
            };

            var agentMessage = await _threadService.SendMessageAsync(sendMessageRequest);
            return Ok(agentMessage);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

