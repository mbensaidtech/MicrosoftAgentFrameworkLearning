using Microsoft.AspNetCore.Mvc;
using AzureAIFoundryFileSearch.Services;
using Azure.AI.Agents.Persistent;
using System.IO;

namespace AzureAIFoundryFileSearch.Controllers;

/// <summary>
/// Controller for managing files (Datasets) in Azure AI Foundry.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatasetsController : ControllerBase
{
    private readonly IDatasetService _datasetService;

    public DatasetsController(IDatasetService datasetService)
    {
        _datasetService = datasetService ?? throw new ArgumentNullException(nameof(datasetService));
    }

    /// <summary>
    /// Lists all files in the Datasets.
    /// </summary>
    /// <returns>The list of files.</returns>
    [HttpGet]
    public async Task<ActionResult<List<PersistentAgentFileInfo>>> ListFiles()
    {
        try
        {
            var files = await _datasetService.ListFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a file by ID.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>The file information.</returns>
    [HttpGet("{fileId}")]
    public async Task<ActionResult<PersistentAgentFileInfo>> GetFile(string fileId)
    {
        try
        {
            var file = await _datasetService.GetFileAsync(fileId);
            return Ok(file);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Uploads a file to Datasets.
    /// </summary>
    /// <param name="request">The request containing the file path.</param>
    /// <returns>The uploaded file information.</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<PersistentAgentFileInfo>> UploadFile([FromBody] UploadFileRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                return BadRequest("FilePath is required.");
            }

            // Combine the Files folder with the provided file path
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", request.FilePath);
            
            if (!System.IO.File.Exists(fullPath))
            {
                return BadRequest($"File not found: {request.FilePath} (resolved to: {fullPath})");
            }

            var file = await _datasetService.UploadFileAsync(request.FilePath);
            return Ok(file);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a file from Datasets.
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{fileId}")]
    public async Task<ActionResult> DeleteFile(string fileId)
    {
        try
        {
            await _datasetService.DeleteFileAsync(fileId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for uploading a file.
/// </summary>
public class UploadFileRequest
{
    /// <summary>
    /// Gets or sets the path to the file to upload.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}

