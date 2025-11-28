using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Data;

public class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TaskItem>()
            .ToContainer("Tasks")
            .HasPartitionKey(t => t.UserId)
            .HasNoDiscriminator();

        builder.Entity<TaskItem>()
            .Property(t => t.Id)
            .ToJsonProperty("id");

        builder.Entity<TaskItem>()
            .Property(t => t.UserId)
            .ToJsonProperty("userId");
    }


}
