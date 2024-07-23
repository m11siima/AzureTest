
using Azure.Storage.Blobs;
using AzureTest.Services;
using System.Drawing;

namespace AzureTest.BackgroundServices;

public class ImageProcessingService : BackgroundService
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly BlobContainerClient _blobContainerClient;

    public ImageProcessingService(ICosmosDbService cosmosDbService, BlobServiceClient blobServiceClient)
    {
        _cosmosDbService = cosmosDbService;
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("name");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(5));

        var inProgressTasks = await _cosmosDbService.GetInProgressTasks();

        if (inProgressTasks.Any())
        {
            foreach(var task in inProgressTasks)
            {
                var blobClient = _blobContainerClient.GetBlobClient(task.FileName);
                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                var image = Image.FromStream(memoryStream);
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    memoryStream = ms;
                }

                await blobClient.UploadAsync(memoryStream);

                task.State = TaskState.Done;
                await _cosmosDbService.Update(task);

            }
        }
    }
}
