using AccommodationAgent.Models;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace AccommodationAgent.Services;

public class RerankingService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<RerankingService> _logger;

    public RerankingService(IChatClient chatClient, ILogger<RerankingService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    /// <summary>
    /// Rerank accommodations using LLM-based pointwise scoring.
    /// Only returns accommodations with a score greater than 6.
    /// </summary>
    public async Task<List<Accommodation>> RerankAccommodationsAsync(
        List<Accommodation> accommodations,
        string userQuery)
    {
        if (accommodations.Count == 0)
        {
            return accommodations;
        }

        var scoredAccommodations = new List<(Accommodation accommodation, int score)>();

        foreach (var accommodation in accommodations)
        {
            try
            {
                var score = await ScoreAccommodationAsync(accommodation, userQuery);
                if (score > 6)
                {
                    scoredAccommodations.Add((accommodation, score));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to score accommodation {AccommodationName}. Skipping.", accommodation.Name);
            }
        }

        // Sort by score descending
        return scoredAccommodations
            .OrderByDescending(x => x.score)
            .Select(x => x.accommodation)
            .ToList();
    }

    private async Task<int> ScoreAccommodationAsync(Accommodation accommodation, string userQuery)
    {
        var prompt = $@"Rate the following accommodation's relevance to the user query on a scale from 1 to 10.
Only respond with a single number between 1 and 10.

User Query: {userQuery}

Accommodation:
Name: {accommodation.Name}
Type: {accommodation.Type}
Rating: {accommodation.Rating}/5
Price per night: €{accommodation.PricePerNight}
Location: {accommodation.Address}
Amenities: {string.Join(", ", accommodation.Amenities)}
Description: {accommodation.Description}

Relevance Score (1-10):";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant that rates accommodations based on user queries. Respond only with a number from 1 to 10."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: default);
        var scoreText = response.Text?.Trim() ?? "1";

        // Try to parse the score
        if (int.TryParse(scoreText, out var score) && score >= 1 && score <= 10)
        {
            return score;
        }

        _logger.LogWarning("Invalid score received: {ScoreText}. Defaulting to 1.", scoreText);
        return 1;
    }
}
