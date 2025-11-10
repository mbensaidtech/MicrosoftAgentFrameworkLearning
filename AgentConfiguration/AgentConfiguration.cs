namespace AgentConfiguration;

/// <summary>
/// Represents the root configuration for agent settings.
/// </summary>
public class AgentConfiguration
{
    /// <summary>
    /// Gets or sets the name of the environment variable containing the Azure OpenAI deployment name.
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the environment variable containing the Azure OpenAI endpoint.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dictionary of agent configurations, keyed by agent name.
    /// </summary>
    public Dictionary<string, AgentSettings> Agents { get; set; } = new();

    /// <summary>
    /// Gets the actual deployment name from the environment variable.
    /// </summary>
    /// <returns>The deployment name value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public string GetDeploymentName()
    {
        return Environment.GetEnvironmentVariable(DeploymentName)
            ?? throw new InvalidOperationException($"Environment variable '{DeploymentName}' is not set.");
    }

    /// <summary>
    /// Gets the actual endpoint URI from the environment variable.
    /// </summary>
    /// <returns>The endpoint URI value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public string GetEndpoint()
    {
        return Environment.GetEnvironmentVariable(Endpoint)
            ?? throw new InvalidOperationException($"Environment variable '{Endpoint}' is not set.");
    }

    /// <summary>
    /// Gets the endpoint as a Uri object.
    /// </summary>
    /// <returns>The endpoint as a Uri.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set or the URI is invalid.</exception>
    public Uri GetEndpointUri()
    {
        var endpoint = GetEndpoint();
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"The endpoint '{endpoint}' is not a valid URI.");
        }
        return uri;
    }

    /// <summary>
    /// Gets an agent configuration by name.
    /// </summary>
    /// <param name="agentName">The name of the agent.</param>
    /// <returns>The agent settings.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the agent is not found in the configuration.</exception>
    public AgentSettings GetAgent(string agentName)
    {
        if (!Agents.TryGetValue(agentName, out var agentSettings))
        {
            throw new KeyNotFoundException($"Agent '{agentName}' is not found in the configuration.");
        }
        return agentSettings;
    }
}

