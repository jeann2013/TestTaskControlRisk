using Newtonsoft.Json;

namespace TaskManager.Api.Entities;

public class TaskItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("userId")]
    public string UserId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; } = "pending";
    public List<string> Subtasks { get; set; } = new();
}
