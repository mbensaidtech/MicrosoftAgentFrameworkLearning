namespace AzureAIFoundryQuickStart.Models;

/// <summary>
/// Request object for sending a message to an agent.
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// Gets or sets the user's message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conversation context containing thread ID and agent ID (optional).
    /// </summary>
    public ConversationContext? Context { get; set; }

    /// <summary>
    /// Validates the request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the message is null or empty.</exception>
    public static void Validate(SendMessageRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("Message cannot be null or empty.", nameof(request));
        }
    }
}

