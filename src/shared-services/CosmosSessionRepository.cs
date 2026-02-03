using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedServices;

/// <summary>
/// Cosmos DB implementation of <see cref="ISessionRepository"/> for persisting agent sessions.
/// </summary>
public sealed class CosmosSessionRepository : ISessionRepository
{
    private readonly Container _container;
    private readonly ILogger<CosmosSessionRepository> _logger;
    private readonly int _ttl;

    private static readonly ItemRequestOptions s_noContentResponse = new() { EnableContentResponseOnWrite = false };

    public CosmosSessionRepository(
        [FromKeyedServices("sessions")] Container container,
        ILogger<CosmosSessionRepository> logger,
        int ttl=-1)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(logger);

        _container = container;
        _logger = logger;
        _ttl = ttl;
    }

    public async Task<JsonElement?> GetSessionAsync(string key, JsonSerializerOptions? serializationOptions, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            var response = await _container
                .ReadItemAsync<CosmosSessionItem>(key, new PartitionKey(key), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(response.Resource.SerializedThread, serializationOptions);

            _logger.LogDebug("Retrieved session {Key}, RU cost: {RequestCharge}", key, response.RequestCharge);

            return jsonElement;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Session not found: {Key}", key);
            return null;
        }
    }

    public async Task SaveSessionAsync(string key, JsonElement session, JsonSerializerOptions? serializationOptions, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var sessionItem = new CosmosSessionItem
        {
            Id = key,
            ConversationId = key,
            SerializedThread = JsonSerializer.Serialize(session, serializationOptions),
            LastUpdated = DateTime.UtcNow,
            Ttl = _ttl
        };

        var response = await _container
            .UpsertItemAsync(sessionItem, new PartitionKey(key), requestOptions: s_noContentResponse, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _logger.LogDebug("Saved session {Key}, RU cost: {RequestCharge}", key, response.RequestCharge);
    }

    private sealed class CosmosSessionItem
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("conversationId")]
        public required string ConversationId { get; init; }

        [JsonPropertyName("serializedThread")]
        public required string SerializedThread { get; init; }

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; init; } = DateTime.UtcNow;

        [JsonPropertyName("ttl")]
        public int Ttl { get; init; } = -1;
    }
}
