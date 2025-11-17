using Microsoft.AspNetCore.Mvc;
using AzureAIFoundryFileSearch.Services;
using Azure.AI.Agents.Persistent;

namespace AzureAIFoundryFileSearch.Controllers;

/// <summary>
/// Controller for managing vector stores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VectorStoresController : ControllerBase
{
    private readonly IVectorStoreService _vectorStoreService;

    public VectorStoresController(IVectorStoreService vectorStoreService)
    {
        _vectorStoreService = vectorStoreService ?? throw new ArgumentNullException(nameof(vectorStoreService));
    }

    /// <summary>
    /// Gets a vector store by ID.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>The vector store information.</returns>
    [HttpGet("{vectorStoreId}")]
    public async Task<ActionResult<PersistentAgentsVectorStore>> GetVectorStore(string vectorStoreId)
    {
        try
        {
            var vectorStore = await _vectorStoreService.GetVectorStoreAsync(vectorStoreId);
            return Ok(vectorStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new vector store.
    /// </summary>
    /// <param name="request">The request containing the vector store name and optional description.</param>
    /// <returns>The created vector store.</returns>
    [HttpPost]
    public async Task<ActionResult<PersistentAgentsVectorStore>> CreateVectorStore([FromBody] CreateVectorStoreRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            var vectorStore = await _vectorStoreService.CreateVectorStoreAsync(request.Name, request.Description);
            return Ok(vectorStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets all files from a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <returns>The list of files in the vector store.</returns>
    [HttpGet("{vectorStoreId}/files")]
    public async Task<ActionResult<List<VectorStoreFile>>> GetVectorStoreFiles(string vectorStoreId)
    {
        try
        {
            var files = await _vectorStoreService.GetVectorStoreFilesAsync(vectorStoreId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a file to a vector store.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store.</param>
    /// <param name="request">The request containing the file ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpPost("{vectorStoreId}/files")]
    public async Task<ActionResult> AddFileToVectorStore(string vectorStoreId, [FromBody] AddFileToVectorStoreRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FileId))
            {
                return BadRequest("FileId is required.");
            }

            await _vectorStoreService.AddFileToVectorStoreAsync(vectorStoreId, request.FileId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cleans a vector store by removing all files from it.
    /// </summary>
    /// <param name="vectorStoreId">The ID of the vector store to clean.</param>
    /// <param name="request">The request containing options for cleaning.</param>
    /// <returns>No content if successful.</returns>
    [HttpPost("{vectorStoreId}/clean")]
    public async Task<ActionResult> CleanVectorStore(string vectorStoreId, [FromBody] CleanVectorStoreRequest? request = null)
    {
        try
        {
            bool removeFilesFromDatasets = request?.RemoveFilesFromDatasets ?? false;
            await _vectorStoreService.CleanVectorStoreAsync(vectorStoreId, removeFilesFromDatasets);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for creating a vector store.
/// </summary>
public class CreateVectorStoreRequest
{
    /// <summary>
    /// Gets or sets the name of the vector store.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description for the vector store.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Request model for adding a file to a vector store.
/// </summary>
public class AddFileToVectorStoreRequest
{
    /// <summary>
    /// Gets or sets the ID of the file to add.
    /// </summary>
    public string FileId { get; set; } = string.Empty;
}

/// <summary>
/// Request model for cleaning a vector store.
/// </summary>
public class CleanVectorStoreRequest
{
    /// <summary>
    /// Gets or sets a value indicating whether to also remove files from Datasets.
    /// </summary>
    public bool RemoveFilesFromDatasets { get; set; } = false;
}

