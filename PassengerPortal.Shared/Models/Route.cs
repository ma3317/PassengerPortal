using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PassengerPortal.Shared.Models;

public class Route
{
    public int Id { get; set; }
    public Station StartStation { get; set; }
    public Station EndStation { get; set; }
    public TimeSpan Duration { get; set; }
    //[JsonIgnore]
    public IList<Timetable> Timetables { get; set; } = new List<Timetable>();
    
    [NotMapped]
    public DateTime DepartureDateTime { get; set; }

    [NotMapped]
    public DateTime ArrivalDateTime { get; set; }
}