namespace AccommodationAgent.Services;

public interface IGeocodingService
{
    /// <summary>
    /// Geocode an address or landmark to coordinates
    /// </summary>
    /// <param name="query">Address or landmark name to geocode</param>
    /// <returns>Coordinates (latitude, longitude) if found, null otherwise</returns>
    Task<(double Latitude, double Longitude)?> GeocodeAsync(string query);
}
