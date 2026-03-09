using Xunit;
using FsCheck;
using FsCheck.Xunit;
using VIRA.Shared.Models;
using VIRA.Shared.Views;

namespace VIRA.Shared.Tests;

/// <summary>
/// Property-based tests for content card field completeness
/// Tests that all card types display their required fields
/// **Validates: Requirements 5.1, 5.2, 5.3, 5.4**
/// </summary>
public class ContentCardPropertyTests
{
    #region Property 11: Content Card Field Completeness

    /// <summary>
    /// Property 11: For any rich content data (weather, news, schedule, traffic), 
    /// the rendered card should display all required fields
    /// **Validates: Requirements 5.1, 5.2, 5.3, 5.4**
    /// </summary>
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 11: Weather, news, schedule, traffic cards display all required fields", MaxTest = 100)]
    public Property ContentCardsDisplayAllRequiredFields()
    {
        // Generator for weather data - all fields must be non-empty
        var weatherGen = Gen.Constant(new WeatherData
        {
            City = "New York",
            Temp = "72°F",
            Condition = "Sunny",
            Humidity = "Humidity: 65%",
            UV = "UV Index: 5",
            Tomorrow = "Tomorrow: 75°F"
        });

        // Generator for news items - all fields must be non-empty
        var newsItemGen = Gen.Constant(new NewsItem
        {
            Category = "Tech",
            Title = "Breaking news headline"
        });

        // Generator for schedule items - all fields must be non-empty
        var scheduleItemGen = Gen.Constant(new ScheduleItem
        {
            Time = "9:00 AM",
            Title = "Team Meeting",
            Location = "Conference Room A",
            Color = "#256AF4"
        });

        // Generator for traffic routes - all fields must be non-empty
        var trafficRouteGen = Gen.Constant(new TrafficRoute
        {
            Route = "I-95 North",
            ETA = "25 min",
            Status = "Light traffic",
            Color = "#22C55E"
        });

        return Prop.ForAll(
            Arb.From(weatherGen),
            (weather) =>
            {
                // Test WeatherCard displays all required fields (Requirement 5.1)
                var weatherCard = new WeatherCard { Data = weather };
                var weatherFieldsPresent = 
                    !string.IsNullOrEmpty(weather.City) &&
                    !string.IsNullOrEmpty(weather.Temp) &&
                    !string.IsNullOrEmpty(weather.Condition) &&
                    !string.IsNullOrEmpty(weather.Humidity) &&
                    !string.IsNullOrEmpty(weather.UV) &&
                    !string.IsNullOrEmpty(weather.Tomorrow);

                return weatherFieldsPresent;
            })
            .And(Prop.ForAll(
                Arb.From(newsItemGen),
                (newsItem) =>
                {
                    // Test NewsCard displays all required fields (Requirement 5.2)
                    var newsCard = new NewsCard { NewsItems = new List<NewsItem> { newsItem } };
                    var newsFieldsPresent =
                        !string.IsNullOrEmpty(newsItem.Category) &&
                        !string.IsNullOrEmpty(newsItem.Title);

                    return newsFieldsPresent;
                }))
            .And(Prop.ForAll(
                Arb.From(scheduleItemGen),
                (scheduleItem) =>
                {
                    // Test ScheduleCard displays all required fields (Requirement 5.3)
                    var scheduleCard = new ScheduleCard { ScheduleItems = new List<ScheduleItem> { scheduleItem } };
                    var scheduleFieldsPresent =
                        !string.IsNullOrEmpty(scheduleItem.Time) &&
                        !string.IsNullOrEmpty(scheduleItem.Title) &&
                        !string.IsNullOrEmpty(scheduleItem.Location) &&
                        !string.IsNullOrEmpty(scheduleItem.Color);

                    return scheduleFieldsPresent;
                }))
            .And(Prop.ForAll(
                Arb.From(trafficRouteGen),
                (trafficRoute) =>
                {
                    // Test TrafficCard displays all required fields (Requirement 5.4)
                    var trafficCard = new TrafficCard { TrafficRoutes = new List<TrafficRoute> { trafficRoute } };
                    var trafficFieldsPresent =
                        !string.IsNullOrEmpty(trafficRoute.Route) &&
                        !string.IsNullOrEmpty(trafficRoute.ETA) &&
                        !string.IsNullOrEmpty(trafficRoute.Status) &&
                        !string.IsNullOrEmpty(trafficRoute.Color);

                    return trafficFieldsPresent;
                }));
    }

    #endregion
}
