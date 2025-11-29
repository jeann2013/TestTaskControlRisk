using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Repositories;

public class UserRepository
{
    private readonly SqlDbContext _db;

    public UserRepository(SqlDbContext db) => _db = db;

    public async Task<User?> GetByEmail(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetById(int id) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task Add(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}
