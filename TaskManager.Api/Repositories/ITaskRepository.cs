using TaskManager.Api.Entities;

namespace TaskManager.Api.Repositories;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetByUser(string userId);
    Task<TaskItem?> GetById(string id, string userId);
    Task Add(TaskItem item);
    Task Update(TaskItem item);
    Task Delete(TaskItem item);
}
