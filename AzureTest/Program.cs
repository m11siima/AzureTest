using Azure.Storage.Blobs;
using AzureTest.BackgroundServices;
using AzureTest.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(x => new BlobServiceClient("ConnectionString"));
builder.Services.AddSingleton<ICosmosDbService>(x =>
{
    var cosmosClient = new CosmosClient("url", "primaryKey");

    return new CosmosDbService(cosmosClient);
});
builder.Services.AddHostedService<ImageProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
