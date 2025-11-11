namespace AgentConfiguration;

/// <summary>
/// Enumeration of available agent types.
/// </summary>
public enum AgentType
{
    /// <summary>
    /// Global agent - can answer questions about any topic.
    /// </summary>
    GlobalAgent,

    /// <summary>
    /// Geography agent - specialized in geography questions.
    /// </summary>
    GeographyAgent,

    /// <summary>
    /// Math agent - specialized in mathematics and problem solving.
    /// </summary>
    MathAgent,

    /// <summary>
    /// Orchestrator agent - coordinates and manages tasks between different agents.
    /// </summary>
    OrchestratorAgent
}

