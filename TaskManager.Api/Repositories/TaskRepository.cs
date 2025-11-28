using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Repositories;

public class TaskRepository
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

    public async Task Add(TaskItem item)
    {
        _context.Tasks.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(TaskItem item)
    {
        _context.Tasks.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(TaskItem item)
    {
        _context.Tasks.Remove(item);
        await _context.SaveChangesAsync();
    }
}
