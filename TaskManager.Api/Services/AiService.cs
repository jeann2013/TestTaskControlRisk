using OpenAI;
using OpenAI.Chat;
using System.Text.Json;

public class AiService
{
    private readonly ChatClient _chatClient;

    public AiService(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"];
        _chatClient = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
    }

    public async Task<object> AnalyzeTask(string text)
    {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Responde SOLO un JSON con propiedades: summary y priority."),
                new UserChatMessage($"Analiza esta tarea: \"{text}\" y devuelve JSON puro con summary y priority.")
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

            string raw = completion.Content[0].Text.Trim();

            
            raw = raw.Replace("```json", "")
                     .Replace("```", "")
                     .Replace("`", "")
                     .Trim();

            
            int start = raw.IndexOf('{');
            int end = raw.LastIndexOf('}');

            if (start == -1 || end == -1)
            {
                return new
                {
                    summary = "Error: AI did not return JSON",
                    priority = "media"
                };
            }

            string jsonText = raw.Substring(start, end - start + 1);

            JsonElement json;

            try
            {
                json = JsonDocument.Parse(jsonText).RootElement;
            }
            catch (Exception ex)
            {
                return new
                {
                    summary = $"Error parsing: {ex.Message}",
                    priority = "media"
                };
            }

            return new
            {
                summary = json.GetProperty("summary").GetString(),
                priority = json.GetProperty("priority").GetString()
            };
        }

    public async Task<object> GenerateSubtasks(string text)
    {
        var messages = new List<ChatMessage>
    {
        new SystemChatMessage("Responde SOLO un array JSON de subtareas."),
        new UserChatMessage($"Genera 4 subtareas para: \"{text}\". Responde SOLO un array JSON puro.")
    };

        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

        string raw = completion.Content[0].Text.Trim();

        // 1. Quitar markdown, backticks, texto extra
        raw = raw.Replace("```json", "")
                 .Replace("```", "")
                 .Replace("`", "")
                 .Trim();

        // 2. Extraer contenido entre []
        int start = raw.IndexOf('[');
        int end = raw.LastIndexOf(']');

        if (start == -1 || end == -1)
        {
            return new
            {
                subtasks = new List<string> { "Error: IA no devolvió un array válido." }
            };
        }

        string jsonArray = raw.Substring(start, end - start + 1);

        List<string>? subtasks;

        try
        {
            subtasks = JsonSerializer.Deserialize<List<string>>(jsonArray);
        }
        catch
        {
            return new
            {
                subtasks = new List<string> { "Error al parsear array." }
            };
        }

        return new
        {
            subtasks = subtasks ?? new List<string>()
        };
    }


}