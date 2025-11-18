using System.Text.Json.Serialization;

namespace OrchestratorAgent.Models;

public struct AIChatMessage
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}
