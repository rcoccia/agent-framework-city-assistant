using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using ActivitiesAgent.Services;

namespace ActivitiesAgent.Tools;

public class ActivitiesTools
{
    private readonly ActivitiesService _activitiesService;

    public ActivitiesTools(ActivitiesService activitiesService)
    {
        _activitiesService = activitiesService;
    }

    [Description("Get a list of all available activities including museums, theaters, cultural events, and attractions")]
    public string GetAllActivities()
    {
        var activities = _activitiesService.GetAllActivities();
        return JsonSerializer.Serialize(activities);
    }

    [Description("Search for activities by category. Supported categories: museums, theaters, cultural_events, attractions")]
    public string GetActivitiesByCategory(
        [Description("The category to filter activities by (e.g., 'museums', 'theaters', 'cultural_events', 'attractions')")] string category)
    {
        var activities = _activitiesService.GetActivitiesByCategory(category);
        return JsonSerializer.Serialize(activities);
    }

    [Description("Search for activities by name, description, or location using keywords")]
    public string SearchActivities(
        [Description("Search query or keywords to find activities")] string query)
    {
        var activities = _activitiesService.SearchActivities(query);
        return JsonSerializer.Serialize(activities);
    }

    public IEnumerable<AIFunction> GetFunctions()
    {
        yield return AIFunctionFactory.Create(GetAllActivities);
        yield return AIFunctionFactory.Create(GetActivitiesByCategory);
        yield return AIFunctionFactory.Create(SearchActivities);
    }
}
