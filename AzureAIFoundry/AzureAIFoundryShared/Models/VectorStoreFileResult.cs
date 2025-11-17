using System.Text.Json.Serialization;

namespace AzureAIFoundryShared.Models;

/// <summary>
/// Represents a file that was uploaded and associated with a vector store.
/// </summary>
public class VectorStoreFileResult
{
    /// <summary>
    /// Gets or sets the file ID that was uploaded.
    /// </summary>
    [JsonPropertyName("fileId")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filename that was used when uploading the file.
    /// </summary>
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;
}

