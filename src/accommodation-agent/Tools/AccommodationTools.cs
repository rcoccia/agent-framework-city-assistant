using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using AccommodationAgent.Services;
using AccommodationAgent.Models;

namespace AccommodationAgent.Tools;

public class AccommodationTools
{
    private readonly IAccommodationService _accommodationService;
    private readonly RerankingService _rerankingService;
    private readonly ILogger<AccommodationTools> _logger;

    // Known landmarks with coordinates
    private static readonly Dictionary<string, (double Latitude, double Longitude)> KnownLandmarks = new(StringComparer.OrdinalIgnoreCase)
    {
        { "colosseum", (41.8902, 12.4922) },
        { "coliseum", (41.8902, 12.4922) },
        { "roman forum", (41.8925, 12.4853) },
        { "vatican", (41.9029, 12.4534) },
        { "pantheon", (41.8986, 12.4768) },
        { "trevi fountain", (41.9009, 12.4833) },
        { "spanish steps", (41.9058, 12.4823) },
        { "trastevere", (41.8899, 12.4707) }
    };

    public AccommodationTools(
        IAccommodationService accommodationService,
        RerankingService rerankingService,
        ILogger<AccommodationTools> logger)
    {
        _accommodationService = accommodationService;
        _rerankingService = rerankingService;
        _logger = logger;
    }

    [Description("Search for accommodations based on various criteria including rating, location, amenities, price, and type")]
    public async Task<string> SearchAccommodationsAsync(
        [Description("The user's original search query to use for reranking results")] string userQuery,
        [Description("Minimum user rating from 1 to 5 (e.g., 4 means at least 4 stars)")] double? minRating = null,
        [Description("City name to search in (e.g., 'Rome', 'Latina')")] string? city = null,
        [Description("Landmark or place name to search near (e.g., 'Colosseum', 'Vatican')")] string? nearLandmark = null,
        [Description("Maximum distance from the landmark in kilometers")] double? maxDistanceKm = null,
        [Description("List of required amenities (all must be present). Options include: parking, room-service, breakfast, wifi, gym, spa, restaurant, pool, bar, air-conditioning, 24-hour-reception, concierge, shared-kitchen")] List<string>? amenities = null,
        [Description("Maximum price per night in euros")] decimal? maxPricePerNight = null,
        [Description("Type of accommodation (e.g., 'hotel', 'bed-and-breakfast', 'hostel')")] string? type = null)
    {
        try
        {
            double? latitude = null;
            double? longitude = null;

            // If searching near a landmark, get its coordinates
            if (!string.IsNullOrWhiteSpace(nearLandmark))
            {
                if (KnownLandmarks.TryGetValue(nearLandmark, out var coordinates))
                {
                    latitude = coordinates.Latitude;
                    longitude = coordinates.Longitude;
                    _logger.LogInformation("Found coordinates for landmark {Landmark}: {Lat}, {Lon}", nearLandmark, latitude, longitude);
                }
                else
                {
                    _logger.LogWarning("Unknown landmark: {Landmark}. Ignoring location filter.", nearLandmark);
                }
            }

            // Search accommodations with filters
            var accommodations = _accommodationService.SearchAccommodations(
                minRating: minRating,
                city: city,
                latitude: latitude,
                longitude: longitude,
                maxDistanceKm: maxDistanceKm,
                amenities: amenities,
                maxPricePerNight: maxPricePerNight,
                type: type);

            if (accommodations.Count == 0)
            {
                return JsonSerializer.Serialize(new { message = "No accommodations found matching the criteria." });
            }

            // Rerank using LLM to return only the most relevant results
            var rerankedAccommodations = await _rerankingService.RerankAccommodationsAsync(accommodations, userQuery);

            if (rerankedAccommodations.Count == 0)
            {
                return JsonSerializer.Serialize(new { 
                    message = "Found some accommodations but none were highly relevant to your query.",
                    matchedCount = accommodations.Count
                });
            }

            return JsonSerializer.Serialize(new
            {
                message = $"Found {rerankedAccommodations.Count} highly relevant accommodation(s) out of {accommodations.Count} matching your criteria.",
                accommodations = rerankedAccommodations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching accommodations");
            return JsonSerializer.Serialize(new { error = "An error occurred while searching for accommodations." });
        }
    }

    [Description("Get all available accommodations without any filters")]
    public string GetAllAccommodations()
    {
        try
        {
            var accommodations = _accommodationService.GetAllAccommodations();
            return JsonSerializer.Serialize(new
            {
                message = $"Found {accommodations.Count} total accommodation(s).",
                accommodations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all accommodations");
            return JsonSerializer.Serialize(new { error = "An error occurred while retrieving accommodations." });
        }
    }

    public IEnumerable<AIFunction> GetFunctions()
    {
        yield return AIFunctionFactory.Create(SearchAccommodationsAsync);
        yield return AIFunctionFactory.Create(GetAllAccommodations);
    }
}
