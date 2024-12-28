using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Services;

public class CompositeFilter : IConnectionFilter
{
    private readonly List<IConnectionFilter> _filters = new List<IConnectionFilter>();

    public void AddFilter(IConnectionFilter filter)
    {
        _filters.Add(filter);
    }

    public IEnumerable<Connection> Apply(IEnumerable<Connection> connections)
    {
        foreach (var filter in _filters)
        {
            connections = filter.Apply(connections);
        }
        return connections;
    }
}
