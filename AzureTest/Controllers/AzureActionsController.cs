using Azure.Storage.Blobs;
using AzureTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AzureActionsController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ICosmosDbService _cosmosDbService;

    public AzureActionsController(BlobServiceClient blobServiceClient, ICosmosDbService cosmosDbService)
    {
        _blobServiceClient = blobServiceClient;
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("name");
        _cosmosDbService = cosmosDbService;
    }

    [HttpPost("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var taskId = Guid.NewGuid();

        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(file.FileName);
            await blobClient.UploadAsync(file.OpenReadStream(), true);
            await _cosmosDbService.Insert(new() { FileName = file.FileName, State = TaskState.Created, TaskId = taskId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(taskId);
    }

    [HttpGet("GetTaskState")]
    public async Task<IActionResult> GetTaskState([FromQuery]Guid taskId)
    {
        try
        {
            var taskModel = _cosmosDbService.Get(taskId);
            return Ok(taskModel.Status);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
