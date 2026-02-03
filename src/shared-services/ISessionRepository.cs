using System.Text.Json;

namespace SharedServices;

/// <summary>
/// Repository interface for persisting agent conversation sessions.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Retrieves a session by its unique key.
    /// </summary>
    /// <param name="key">The unique session key.</param>
    /// <param name="serializationOptions">JSON serialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session content if found; otherwise, <c>null</c>.</returns>
    Task<JsonElement?> GetSessionAsync(string key, JsonSerializerOptions? serializationOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates a session.
    /// </summary>
    /// <param name="key">The unique session key.</param>
    /// <param name="session">The session content to persist.</param>
    /// <param name="serializationOptions">JSON serialization options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveSessionAsync(string key, JsonElement session, JsonSerializerOptions? serializationOptions, CancellationToken cancellationToken = default);
}
