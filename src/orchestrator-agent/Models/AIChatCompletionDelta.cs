using System.Text.Json.Serialization;

namespace OrchestratorAgent.Models;

public record AIChatCompletionDelta([property: JsonPropertyName("delta")] AIChatMessageDelta Delta)
{
    [JsonInclude, JsonPropertyName("sessionState"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SessionState;
}
