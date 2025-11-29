using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Entities;
using TaskManager.Api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly CosmosDbContext _context;

    public TaskRepository(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetByUser(string userId)
    {
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetById(string id, string userId)
    {
        return await _context.Tasks
            .Where(t => t.Id == id && t.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task Add(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task Update(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}
