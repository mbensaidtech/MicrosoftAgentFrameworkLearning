namespace AgentConfiguration;

/// <summary>
/// Represents vector stores configuration for an agent's tools.
/// </summary>
public class AgentVectorStoresSettings
{
    /// <summary>
    /// Gets or sets the vector store ID to use with this agent.
    /// </summary>
    public string? VectorStoreId { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to return from file search. Default is 10.
    /// </summary>
    public int MaxNumResults { get; set; } = 10;
}

