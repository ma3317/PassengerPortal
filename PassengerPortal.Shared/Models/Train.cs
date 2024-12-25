namespace PassengerPortal.Shared.Models;

public class Train
{
    public int Id { get; set; }
    public string TrainNumber { get; set; }
    public IList<Route> RoutesCovered { get; set; } = new List<Route>();
}