using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Api.DTOs;
using TaskManager.Api.Entities;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskRepository _tasks;
    private readonly AiService _ai;

    public TasksController(TaskRepository tasks, AiService ai)
    {
        _tasks = tasks;
        _ai = ai;
    }

    private string UserId =>
    User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
    ?? throw new Exception("UserId claim missing");


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _tasks.GetByUser(UserId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskDto dto)
    {

        var task = new TaskItem
        {
            UserId = UserId,
            Title = dto.Title,
            Description = dto.Description
        };

        await _tasks.Add(task);
        return Ok(task);
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze(TaskAnalysisRequest dto)
    {
        var result = await _ai.AnalyzeTask(dto.Text);
        return Ok(result);
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> Suggest(TaskAnalysisRequest dto)
    {
        var result = await _ai.GenerateSubtasks(dto.Text);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TaskUpdateDto dto)
    {
        var task = await _tasks.GetById(id, UserId);
        if (task == null)
            return NotFound(new { message = "Task not found" });

        task.Title = dto.Title ?? task.Title;
        task.Description = dto.Description ?? task.Description;
        task.Priority = dto.Priority ?? task.Priority;

        await _tasks.Update(task);

        return Ok(task);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var task = await _tasks.GetById(id, UserId);
        if (task == null)
            return NotFound(new { message = "Task not found" });

        await _tasks.Delete(task);

        return Ok(new { message = "Deleted successfully" });
    }

    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Complete(string id)
    {
        var task = await _tasks.GetById(id, UserId);
        if (task == null)
            return NotFound();

        task.Priority = "done";
        await _tasks.Update(task);

        return Ok(task);
    }



}
