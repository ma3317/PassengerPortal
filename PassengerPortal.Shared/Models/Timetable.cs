using System.Text.Json.Serialization;

namespace PassengerPortal.Shared.Models;
public class Timetable
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    [JsonIgnore]
    public Route Route { get; set; }

    // Zmieniamy z DateTime -> TimeSpan
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
}
