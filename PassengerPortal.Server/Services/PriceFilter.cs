using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Services;

public class PriceFilter : IConnectionFilter
{
    private readonly decimal _maximumPrice;

    public PriceFilter(decimal maximumPrice)
    {
        _maximumPrice = maximumPrice;
    }

    public IEnumerable<Connection> Apply(IEnumerable<Connection> connections)
    {
        return connections.Where(c => c.TotalPrice <= _maximumPrice);
    }
}
