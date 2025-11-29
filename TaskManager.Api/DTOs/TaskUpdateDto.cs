namespace TaskManager.Api.DTOs;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
}
