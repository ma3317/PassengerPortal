using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Services;

public class AvailableSeatsFilter : IConnectionFilter
{
    private readonly int _minimumSeats;

    public AvailableSeatsFilter(int minimumSeats)
    {
        _minimumSeats = minimumSeats;
    }

    public IEnumerable<Connection> Apply(IEnumerable<Connection> connections)
    {
        return connections.Where(c => c.Routes.All(r => r.AvailableSeats >= _minimumSeats));
    }
}
