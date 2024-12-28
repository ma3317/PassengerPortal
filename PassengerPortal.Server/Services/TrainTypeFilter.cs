using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Services;

public class TrainTypeFilter : IConnectionFilter
{
    private readonly TrainType _trainType;

    public TrainTypeFilter(TrainType trainType)
    {
        _trainType = trainType;
    }

    public IEnumerable<Connection> Apply(IEnumerable<Connection> connections)
    {
        return connections.Where(c => c.Routes.Any(r => r.TrainType == _trainType));
    }
}
