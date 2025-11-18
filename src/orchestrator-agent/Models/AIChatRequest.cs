using System.Text.Json.Serialization;

namespace OrchestratorAgent.Models;

public record AIChatRequest([property: JsonPropertyName("messages")] IList<AIChatMessage> Messages)
{
    [JsonInclude, JsonPropertyName("sessionState")]
    public string? SessionState;
}
