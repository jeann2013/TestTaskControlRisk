namespace TaskManager.Api.Services;

public class AiService
{
    public async Task<object> AnalyzeTask(string text)
    {
        return new
        {
            priority = "medium",
            summary = text.Length > 50 ? text[..50] + "..." : text
        };
    }

    public async Task<object> GenerateSubtasks(string text)
    {
        return new
        {
            subtasks = new[] { "Paso 1", "Paso 2", "Paso 3" }
        };
    }
}
