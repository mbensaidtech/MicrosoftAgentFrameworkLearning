namespace AgentConfiguration;

/// <summary>
/// Represents the configuration settings for an individual agent.
/// </summary>
public class AgentSettings
{
    /// <summary>
    /// Gets or sets the name of the agent.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the instructions for the agent.
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tools configuration for the agent.
    /// </summary>
    public AgentToolsSettings? Tools { get; set; }
}

