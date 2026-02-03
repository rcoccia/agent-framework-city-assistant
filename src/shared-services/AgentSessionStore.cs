using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SharedServices;

/// <summary>
/// Cosmos DB-backed implementation of <see cref="AgentSessionStore"/> for persisting agent conversation sessions.
/// </summary>
public sealed class AgentSessionStore : Microsoft.Agents.AI.Hosting.AgentSessionStore
{
    private readonly ISessionRepository _repository;
    private readonly ILogger<AgentSessionStore> _logger;
    private readonly JsonSerializerOptions _serializationOptions;

    /// <summary>
    /// JSON options that support case-insensitive property matching.
    /// This is needed because the MAF framework serializes SessionState with camelCase
    /// but deserializes expecting PascalCase.
    /// </summary>
    private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public AgentSessionStore(
        ISessionRepository repository,
        ILogger<AgentSessionStore> logger,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);

        _repository = repository;
        _logger = logger;
        _serializationOptions = jsonSerializerOptions ?? s_caseInsensitiveOptions;
    }

    public override async ValueTask<AgentSession> GetSessionAsync(
        AIAgent agent,
        string conversationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentException.ThrowIfNullOrWhiteSpace(conversationId);

        var key = GetKey(conversationId, agent.Id);

        _logger.LogDebug("Retrieving session for conversation {ConversationId}, agent {AgentId}", conversationId, agent.Id);

        var sessionContent = await _repository
            .GetSessionAsync(key, _serializationOptions, cancellationToken)
            .ConfigureAwait(false);

        if (sessionContent is null)
        {
            _logger.LogDebug("No existing session found, creating new session for {ConversationId}", conversationId);
            return await agent.GetNewSessionAsync(cancellationToken).ConfigureAwait(false);

        }

        // DEBUG: Log the raw JSON being deserialized
        _logger.LogWarning("DEBUG - Raw JSON from Cosmos: {Json}", sessionContent.Value.GetRawText());

        var session = await agent
            .DeserializeSessionAsync(sessionContent.Value, jsonSerializerOptions: _serializationOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // DEBUG: Log what we got back
        var chatHistoryProvider = session.GetService<ChatHistoryProvider>();
        _logger.LogWarning("DEBUG - After deserialize: ChatHistoryProvider is {Type}", 
            chatHistoryProvider?.GetType().Name ?? "NULL");

        return session;
    }

    public override async ValueTask SaveSessionAsync(
        AIAgent agent,
        string conversationId,
        AgentSession session,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentException.ThrowIfNullOrWhiteSpace(conversationId);
        ArgumentNullException.ThrowIfNull(session);

        var key = GetKey(conversationId, agent.Id);
        var serializedThread = session.Serialize(_serializationOptions);

        // DEBUG: Log the JSON being saved
        _logger.LogWarning("DEBUG - JSON being saved to Cosmos: {Json}", serializedThread.GetRawText());

        _logger.LogDebug("Saving session for conversation {ConversationId}, agent {AgentId}", conversationId, agent.Id);

        await _repository
            .SaveSessionAsync(key, serializedThread, _serializationOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private static string GetKey(string conversationId, string agentId) => $"{agentId}:{conversationId}";
}
