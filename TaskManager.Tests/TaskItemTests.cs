using TaskManager.Api.Entities;
using Xunit;

namespace TaskManager.Tests.Entities;

public class TaskItemTests
{
    [Fact]
    public void Constructor_ShouldGenerateId()
    {
        var task = new TaskItem();

        Assert.False(string.IsNullOrWhiteSpace(task.Id));
    }

    [Fact]
    public void Priority_Default_ShouldBePending()
    {
        var task = new TaskItem();

        Assert.Equal("pending", task.Priority);
    }

    [Fact]
    public void Subtasks_ShouldStartEmpty()
    {
        var task = new TaskItem();

        Assert.NotNull(task.Subtasks);
        Assert.Empty(task.Subtasks);
    }

    [Fact]
    public void ShouldAssignUserIdTitleAndDescription()
    {
        var task = new TaskItem
        {
            UserId = "user1",
            Title = "Test title",
            Description = "Test description"
        };

        Assert.Equal("user1", task.UserId);
        Assert.Equal("Test title", task.Title);
        Assert.Equal("Test description", task.Description);
    }

    [Fact]
    public void ShouldAllowUpdatingFields()
    {
        var task = new TaskItem
        {
            UserId = "user",
            Title = "Old",
            Description = "Old",
            Priority = "pending"
        };

        task.Title = "New";
        task.Description = "New desc";
        task.Priority = "urgent";

        Assert.Equal("New", task.Title);
        Assert.Equal("New desc", task.Description);
        Assert.Equal("urgent", task.Priority);
    }

    [Fact]
    public void ShouldAddSubtasks()
    {
        var task = new TaskItem();

        task.Subtasks.Add("Step 1");
        task.Subtasks.Add("Step 2");

        Assert.Equal(2, task.Subtasks.Count);
    }
}
