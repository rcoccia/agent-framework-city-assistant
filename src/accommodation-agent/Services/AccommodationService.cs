using AccommodationAgent.Models;

namespace AccommodationAgent.Services;

public class AccommodationService : IAccommodationService
{
    private readonly List<Accommodation> _accommodations;

    public AccommodationService()
    {
        _accommodations = new List<Accommodation>
        {
            // Hotels in Rome near Colosseum
            new Accommodation
            {
                Id = "1",
                Name = "Grand Hotel Colosseo",
                Type = "hotel",
                Rating = 4.8,
                Amenities = ["parking", "room-service", "breakfast", "wifi", "gym", "restaurant"],
                Position = new Location { Latitude = 41.8902, Longitude = 12.4922 },
                Address = new Address
                {
                    Street = "Via Labicana 125",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00184",
                    Country = "Italy"
                },
                PricePerNight = 180.00m,
                Description = "Luxury hotel near the Colosseum with stunning views and excellent amenities."
            },
            new Accommodation
            {
                Id = "2",
                Name = "Hotel Forum",
                Type = "hotel",
                Rating = 4.6,
                Amenities = ["breakfast", "wifi", "restaurant", "bar"],
                Position = new Location { Latitude = 41.8925, Longitude = 12.4853 },
                Address = new Address
                {
                    Street = "Via Tor de' Conti 25",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00184",
                    Country = "Italy"
                },
                PricePerNight = 150.00m,
                Description = "Charming hotel overlooking the Roman Forum with rooftop terrace."
            },
            new Accommodation
            {
                Id = "3",
                Name = "Palazzo Manfredi",
                Type = "hotel",
                Rating = 4.9,
                Amenities = ["parking", "room-service", "breakfast", "wifi", "gym", "spa", "restaurant", "pool"],
                Position = new Location { Latitude = 41.8897, Longitude = 12.4964 },
                Address = new Address
                {
                    Street = "Via Labicana 125",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00184",
                    Country = "Italy"
                },
                PricePerNight = 450.00m,
                Description = "Five-star luxury hotel with exclusive Colosseum views and Michelin-starred restaurant."
            },
            
            // B&Bs in Rome
            new Accommodation
            {
                Id = "4",
                Name = "Colosseum B&B",
                Type = "bed-and-breakfast",
                Rating = 4.5,
                Amenities = ["breakfast", "wifi", "parking"],
                Position = new Location { Latitude = 41.8905, Longitude = 12.4930 },
                Address = new Address
                {
                    Street = "Via Capo d'Africa 21",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00184",
                    Country = "Italy"
                },
                PricePerNight = 75.00m,
                Description = "Cozy B&B just steps from the Colosseum with homemade breakfast."
            },
            new Accommodation
            {
                Id = "5",
                Name = "Trastevere Hideaway",
                Type = "bed-and-breakfast",
                Rating = 4.7,
                Amenities = ["breakfast", "wifi", "air-conditioning"],
                Position = new Location { Latitude = 41.8899, Longitude = 12.4707 },
                Address = new Address
                {
                    Street = "Via della Paglia 15",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00153",
                    Country = "Italy"
                },
                PricePerNight = 65.00m,
                Description = "Charming B&B in the heart of Trastevere with authentic Roman hospitality."
            },
            
            // Hotels in Latina
            new Accommodation
            {
                Id = "6",
                Name = "Hotel Latina",
                Type = "hotel",
                Rating = 4.2,
                Amenities = ["breakfast", "wifi", "parking", "restaurant", "bar"],
                Position = new Location { Latitude = 41.4677, Longitude = 12.9037 },
                Address = new Address
                {
                    Street = "Viale Kennedy 50",
                    City = "Latina",
                    State = "Lazio",
                    ZipCode = "04100",
                    Country = "Italy"
                },
                PricePerNight = 85.00m,
                Description = "Modern hotel in Latina city center with comfortable rooms and good amenities."
            },
            new Accommodation
            {
                Id = "7",
                Name = "Park Hotel",
                Type = "hotel",
                Rating = 4.4,
                Amenities = ["breakfast", "wifi", "parking", "gym", "restaurant", "pool"],
                Position = new Location { Latitude = 41.4701, Longitude = 12.9049 },
                Address = new Address
                {
                    Street = "Via Isonzo 45",
                    City = "Latina",
                    State = "Lazio",
                    ZipCode = "04100",
                    Country = "Italy"
                },
                PricePerNight = 110.00m,
                Description = "Elegant hotel with pool and spa facilities in Latina."
            },
            
            // Budget-friendly options
            new Accommodation
            {
                Id = "8",
                Name = "Hostel Roma",
                Type = "hostel",
                Rating = 4.0,
                Amenities = ["wifi", "breakfast", "shared-kitchen"],
                Position = new Location { Latitude = 41.9028, Longitude = 12.4964 },
                Address = new Address
                {
                    Street = "Via Castro Pretorio 25",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00185",
                    Country = "Italy"
                },
                PricePerNight = 30.00m,
                Description = "Budget-friendly hostel near Termini station with clean facilities."
            },
            new Accommodation
            {
                Id = "9",
                Name = "Budget Inn Rome",
                Type = "hotel",
                Rating = 3.8,
                Amenities = ["wifi", "breakfast"],
                Position = new Location { Latitude = 41.8980, Longitude = 12.4872 },
                Address = new Address
                {
                    Street = "Via Nazionale 230",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00184",
                    Country = "Italy"
                },
                PricePerNight = 45.00m,
                Description = "Affordable hotel with basic amenities in central Rome."
            },
            
            // More variety
            new Accommodation
            {
                Id = "10",
                Name = "Vatican View B&B",
                Type = "bed-and-breakfast",
                Rating = 4.6,
                Amenities = ["breakfast", "wifi", "parking", "air-conditioning"],
                Position = new Location { Latitude = 41.9029, Longitude = 12.4534 },
                Address = new Address
                {
                    Street = "Via Germanico 198",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00192",
                    Country = "Italy"
                },
                PricePerNight = 80.00m,
                Description = "Lovely B&B near Vatican City with stunning views and great breakfast."
            },
            new Accommodation
            {
                Id = "11",
                Name = "Pantheon Suites",
                Type = "hotel",
                Rating = 4.8,
                Amenities = ["wifi", "room-service", "breakfast", "bar", "concierge"],
                Position = new Location { Latitude = 41.8986, Longitude = 12.4768 },
                Address = new Address
                {
                    Street = "Piazza della Rotonda 73",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00186",
                    Country = "Italy"
                },
                PricePerNight = 280.00m,
                Description = "Boutique hotel right on the Pantheon square with elegant rooms."
            },
            new Accommodation
            {
                Id = "12",
                Name = "Termini Budget Hotel",
                Type = "hotel",
                Rating = 3.9,
                Amenities = ["wifi", "breakfast", "24-hour-reception"],
                Position = new Location { Latitude = 41.9008, Longitude = 12.5015 },
                Address = new Address
                {
                    Street = "Via Marsala 80",
                    City = "Rome",
                    State = "Lazio",
                    ZipCode = "00185",
                    Country = "Italy"
                },
                PricePerNight = 55.00m,
                Description = "Simple hotel near Termini station, perfect for short stays."
            }
        };
    }

    public List<Accommodation> GetAllAccommodations()
    {
        return _accommodations;
    }

    public List<Accommodation> SearchAccommodations(
        double? minRating = null,
        string? city = null,
        double? latitude = null,
        double? longitude = null,
        double? maxDistanceKm = null,
        List<string>? amenities = null,
        decimal? maxPricePerNight = null,
        string? type = null)
    {
        var results = _accommodations.AsQueryable();

        // Filter by rating
        if (minRating.HasValue)
        {
            results = results.Where(a => a.Rating >= minRating.Value);
        }

        // Filter by city
        if (!string.IsNullOrWhiteSpace(city))
        {
            results = results.Where(a => a.Address.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by distance from a reference point
        if (latitude.HasValue && longitude.HasValue && maxDistanceKm.HasValue)
        {
            results = results.Where(a =>
                CalculateDistance(latitude.Value, longitude.Value, a.Position.Latitude, a.Position.Longitude) <= maxDistanceKm.Value);
        }

        // Filter by amenities (all amenities must be present)
        if (amenities != null && amenities.Count > 0)
        {
            results = results.Where(a =>
                amenities.All(amenity => a.Amenities.Contains(amenity, StringComparer.OrdinalIgnoreCase)));
        }

        // Filter by max price per night
        if (maxPricePerNight.HasValue)
        {
            results = results.Where(a => a.PricePerNight <= maxPricePerNight.Value);
        }

        // Filter by type
        if (!string.IsNullOrWhiteSpace(type))
        {
            results = results.Where(a => a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        return results.ToList();
    }

    /// <summary>
    /// Calculate distance between two coordinates using Haversine formula
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
