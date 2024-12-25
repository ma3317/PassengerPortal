namespace PassengerPortal.Shared.Models;

public class Route
{
    public int Id { get; set; }
    public Station StartStation { get; set; }
    public Station EndStation { get; set; }
    public TimeSpan Duration { get; set; }
    public IList<Timetable> Timetables { get; set; } = new List<Timetable>();
}