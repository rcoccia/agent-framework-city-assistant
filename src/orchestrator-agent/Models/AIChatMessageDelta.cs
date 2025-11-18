using System.Text.Json.Serialization;

namespace OrchestratorAgent.Models;

public struct AIChatMessageDelta
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
