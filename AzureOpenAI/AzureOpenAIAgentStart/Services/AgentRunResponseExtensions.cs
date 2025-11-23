using AzureOpenAIAgentStart.Models;
using Microsoft.Agents.AI;
using System.Linq;
using static CommonUtilities.ColoredConsole;

namespace AzureOpenAIAgentStart.Services;

/// <summary>
/// Extension methods for AgentRunResponse.
/// </summary>
public static class AgentRunResponseExtensions
{
    /// <summary>
    /// Converts an AgentRunResponse to an AgentResponse.
    /// </summary>
    /// <param name="agentRunResponse">The agent run response to convert.</param>
    /// <returns>An AgentResponse containing the mapped data.</returns>
    public static AgentResponse ToAgentResponse(this AgentRunResponse agentRunResponse)
    {
        if (agentRunResponse == null)
        {
            throw new ArgumentNullException(nameof(agentRunResponse));
        }

        return new AgentResponse
        {
            AgentId = agentRunResponse.AgentId ?? string.Empty,
            CreatedAt = agentRunResponse.CreatedAt?.DateTime ?? DateTime.UtcNow,
            Usage = agentRunResponse.Usage != null ? new TokenUsage
            {
                InputTokenCount = (int)(agentRunResponse.Usage.InputTokenCount ?? 0),
                OutputTokenCount = (int)(agentRunResponse.Usage.OutputTokenCount ?? 0),
                TotalTokenCount = (int)(agentRunResponse.Usage.TotalTokenCount ?? 0),
                AdditionalCounts = agentRunResponse.Usage.AdditionalCounts != null
                    ? agentRunResponse.Usage.AdditionalCounts.ToDictionary(kvp => kvp.Key, kvp => (int)kvp.Value)
                    : null
            } : null,
            Messages = agentRunResponse.Messages?.Select(msg => new Message
            {
                AuthorName = msg.AuthorName ?? string.Empty,
                CreatedAt = msg.CreatedAt?.DateTime ?? DateTime.UtcNow,
                Role = MapMessageRole(msg.Role.ToString()),
                MessageId = msg.MessageId ?? string.Empty,
                Content = ExtractTextContent(msg.Contents)
            }).ToList() ?? new List<Message>()
        };
    }

    /// <summary>
    /// Maps the Microsoft.Agents.AI message role to the MessageRole enum.
    /// </summary>
    private static MessageRole MapMessageRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return MessageRole.User;

        return role.ToLowerInvariant() switch
        {
            "assistant" => MessageRole.Assistant,
            "system" => MessageRole.System,
            "tool" => MessageRole.Tool,
            "user" => MessageRole.User,
            _ => MessageRole.User
        };
    }

    /// <summary>
    /// Extracts text content from message contents collection.
    /// </summary>
    private static string ExtractTextContent(dynamic? contents)
    {
        if (contents == null)
            return string.Empty;

        try
        {
            var contentsList = contents as System.Collections.IEnumerable;
            if (contentsList == null)
                return string.Empty;

            var textParts = new List<string>();
            foreach (var item in contentsList)
            {
                if (item == null)
                    continue;

                // Try to get Text property using reflection
                var textProperty = item.GetType().GetProperty("Text");
                if (textProperty != null)
                {
                    var textValue = textProperty.GetValue(item)?.ToString();
                    if (!string.IsNullOrWhiteSpace(textValue))
                    {
                        textParts.Add(textValue);
                    }
                }
                else
                {
                    var textValue = item.ToString();
                    if (!string.IsNullOrWhiteSpace(textValue) && textValue != item.GetType().FullName)
                    {
                        textParts.Add(textValue);
                    }
                }
            }

            return string.Join(" ", textParts);
        }
        catch
        {
            // Fallback to ToString if dynamic access fails
            return contents?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Logs the token usage information from the agent run response with a header and separator lines.
    /// </summary>
    /// <param name="agentRunResponse">The agent run response to log token usage for.</param>
    public static void LogTokenUsage(this AgentRunResponse agentRunResponse)
    {
        if (agentRunResponse == null)
        {
            return;
        }

        WriteDividerLine();
        WritePrimaryLogLine("Token Usage");
        WriteSecondaryLogLine($"- Input Tokens: {agentRunResponse.Usage?.InputTokenCount}");
        WriteSecondaryLogLine($"- Output Tokens: {agentRunResponse.Usage?.OutputTokenCount}");
        WriteDividerLine();
    }
}

