namespace AzureOpenAIShared;

/// <summary>
/// Attribute to mark methods as dangerous, which will exclude them from being converted to AI tools.
/// Use this attribute on methods that should not be exposed to the AI agent for safety reasons.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class DangerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DangerAttribute"/> class.
    /// </summary>
    public DangerAttribute()
    {
    }

    /// <summary>
    /// Gets or sets an optional reason why this method is marked as dangerous.
    /// </summary>
    public string? Reason { get; set; }
}

