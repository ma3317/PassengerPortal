using PassengerPortal.Shared.Models;

namespace PassengerPortal.Shared.Interfaces;

public interface IConnectionFilter
{
    IEnumerable<Connection> Apply(IEnumerable<Connection> connections);
}
