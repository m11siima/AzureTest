using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace AzureTest.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;
    
    public CosmosDbService(CosmosClient client)
    {
        _container = client.GetContainer("name", "containerName");
    }

    public async Task<TaskModel> Get(Guid taskId)
    {
        var query = _container.GetItemQueryIterator<TaskModel>(new QueryDefinition($"SELECT * FROM Task WHERE Id = {taskId}"));

        var result = await query.ReadNextAsync();

        return result.First();
    }

    public async Task<List<TaskModel>> GetInProgressTasks()
    {
        var query = _container.GetItemQueryIterator<TaskModel>(new QueryDefinition("SELECT * FROM Task WHERE Status = 1 OR Status = 2"));

        var result = new List<TaskModel>();

        while(query.HasMoreResults) 
        {
            result.AddRange(await query.ReadNextAsync());
        }

        return result;
    } 

    public async Task Insert(TaskModel taskModel)
    {
        var item = await _container.CreateItemAsync(taskModel);
    }

    public async Task Update(TaskModel taskModel)
    {
        var item = await _container.UpsertItemAsync<TaskModel>(taskModel);
    }
}
