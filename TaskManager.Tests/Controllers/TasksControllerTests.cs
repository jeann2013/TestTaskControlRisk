using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManager.Api.Controllers;
using TaskManager.Api.DTOs;
using TaskManager.Api.Entities;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;
using Xunit;

public class TasksControllerTests
{
    private TasksController CreateController(
        Mock<ITaskRepository> repoMock,
        Mock<AiService>? aiMock = null,
        string userId = "user123",
        string roles = "User")
    {
        aiMock ??= new Mock<AiService>();

        var controller = new TasksController(repoMock.Object, aiMock.Object);

        // Fake user identity
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, roles)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Fact]
    public async Task Get_ReturnsTasksForUser()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var ai = new Mock<AiService>();

        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = "1", UserId = "user123", Title = "Test Task" }
        };

        repo.Setup(r => r.GetByUser("user123")).ReturnsAsync(tasks);

        var controller = CreateController(repo);

        // Act
        var result = await controller.Get();
        var ok = Assert.IsType<OkObjectResult>(result);

        // Assert
        var returned = Assert.IsAssignableFrom<List<TaskItem>>(ok.Value);
        Assert.Single(returned);
        Assert.Equal("Test Task", returned[0].Title);
    }

    [Fact]
    public async Task Create_AddsTask()
    {
        var repo = new Mock<ITaskRepository>();
        var controller = CreateController(repo);

        var dto = new TaskDto
        {
            Title = "New Task",
            Description = "Description"
        };

        // Act
        var result = await controller.Create(dto);
        var ok = Assert.IsType<OkObjectResult>(result);

        var task = Assert.IsType<TaskItem>(ok.Value);
        Assert.Equal("New Task", task.Title);

        repo.Verify(r => r.Add(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task Update_UpdatesTask()
    {
        var repo = new Mock<ITaskRepository>();

        var existing = new TaskItem
        {
            Id = "task1",
            UserId = "user123",
            Title = "Old",
            Description = "Old Desc"
        };

        repo.Setup(r => r.GetById("task1", "user123")).ReturnsAsync(existing);

        var controller = CreateController(repo);
        var dto = new TaskUpdateDto
        {
            Title = "Updated"
        };

        var result = await controller.Update("task1", dto);
        var ok = Assert.IsType<OkObjectResult>(result);

        var updated = Assert.IsType<TaskItem>(ok.Value);
        Assert.Equal("Updated", updated.Title);

        repo.Verify(r => r.Update(existing), Times.Once);
    }

   
    [Fact]
    public async Task Delete_WorksForAdmin()
    {
        var repo = new Mock<ITaskRepository>();
        var task = new TaskItem { Id = "1", UserId = "user123" };

        repo.Setup(r => r.GetById("1", "user123")).ReturnsAsync(task);

        var controller = CreateController(
            repo,
            userId: "user123",
            roles: "Admin"
        );

        var result = await controller.Delete("1");

        var ok = Assert.IsType<OkObjectResult>(result);
        repo.Verify(r => r.Delete(task), Times.Once);
    }

    [Fact]
    public async Task Complete_SetsTaskToDone()
    {
        var repo = new Mock<ITaskRepository>();
        var task = new TaskItem { Id = "5", UserId = "user123", Priority = "pending" };

        repo.Setup(r => r.GetById("5", "user123")).ReturnsAsync(task);

        var controller = CreateController(
            repo,
            roles: "Admin"
        );

        var result = await controller.Complete("5");

        var ok = Assert.IsType<OkObjectResult>(result);
        var updated = Assert.IsType<TaskItem>(ok.Value);

        Assert.Equal("done", updated.Priority);

        repo.Verify(r => r.Update(task), Times.Once);
    }
}
