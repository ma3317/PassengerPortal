namespace PassengerPortal.Shared.Models;

public class Connection
{
    public int Id { get; set; } // Klucz główny

    public List<Route> Routes { get; set; } = new List<Route>();
    public TimeSpan TotalDuration => CalculateTotalDuration();

    private TimeSpan CalculateTotalDuration()
    {
        if (Routes.Count == 0) return TimeSpan.Zero;
        TimeSpan total = TimeSpan.Zero;
        foreach (var r in Routes)
        {
            total += r.Duration;
        }
        return total;
    }
}