namespace AccommodationAgent.Services;

/// <summary>
/// Mock geocoding service that returns coordinates for known locations.
/// This will be replaced with a real geocoding API (e.g., Azure Maps API) in the future.
/// </summary>
public class GeocodingService : IGeocodingService
{
    private readonly ILogger<GeocodingService> _logger;
    
    // Mock data for known landmarks and cities
    private static readonly Dictionary<string, (double Latitude, double Longitude)> KnownLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        // Rome landmarks
        { "colosseum", (41.8902, 12.4922) },
        { "coliseum", (41.8902, 12.4922) },
        { "roman forum", (41.8925, 12.4853) },
        { "vatican", (41.9029, 12.4534) },
        { "vatican city", (41.9029, 12.4534) },
        { "pantheon", (41.8986, 12.4768) },
        { "trevi fountain", (41.9009, 12.4833) },
        { "spanish steps", (41.9058, 12.4823) },
        { "trastevere", (41.8899, 12.4707) },
        
        // Cities
        { "rome", (41.9028, 12.4964) },
        { "roma", (41.9028, 12.4964) },
        { "latina", (41.4677, 12.9037) },
        
        // Rome areas/neighborhoods
        { "downtown rome", (41.9028, 12.4964) },
        { "rome city center", (41.9028, 12.4964) },
        { "termini station", (41.9008, 12.5015) }
    };

    public GeocodingService(ILogger<GeocodingService> logger)
    {
        _logger = logger;
    }

    public Task<(double Latitude, double Longitude)?> GeocodeAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult<(double Latitude, double Longitude)?>(null);
        }

        if (KnownLocations.TryGetValue(query.Trim(), out var coordinates))
        {
            _logger.LogInformation("Geocoded '{Query}' to coordinates: {Lat}, {Lon}", query, coordinates.Latitude, coordinates.Longitude);
            return Task.FromResult<(double Latitude, double Longitude)?>(coordinates);
        }

        // Generate realistic but random coordinates in the Rome/Lazio area when location not found
        // Rome/Lazio region roughly: Latitude 41.4-42.2, Longitude 12.2-13.5
        var random = new Random(query.GetHashCode()); // Use query hash for deterministic randomness
        var randomLat = 41.4 + (random.NextDouble() * 0.8); // 41.4 to 42.2
        var randomLon = 12.2 + (random.NextDouble() * 1.3); // 12.2 to 13.5
        
        _logger.LogWarning("Location '{Query}' not found in mock data. Returning random coordinates in Lazio region: {Lat}, {Lon}", 
            query, randomLat, randomLon);
        
        return Task.FromResult<(double Latitude, double Longitude)?>((randomLat, randomLon));
    }
}
