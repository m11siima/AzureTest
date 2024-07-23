namespace AzureTest.Services;

public interface ICosmosDbService
{
    public Task Insert(TaskModel taskModel);
    public Task Update(TaskModel taskModel);
    public Task<TaskModel> Get(Guid taskId);
    public Task<List<TaskModel>> GetInProgressTasks();
}

public class TaskModel
{
    public Guid TaskId { get; set; }
    public string FileName { get; set; }
    public TaskState State { get; set; }
}

public enum TaskState
{
    InProgress = 1,
    Created = 2,
    Done = 3,
    Error = 4
}
