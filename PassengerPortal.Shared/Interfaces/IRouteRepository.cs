using PassengerPortal.Shared.Models;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface IRouteRepository
    {
        Route GetById(int id);
        IEnumerable<Route> GetAll();
        IEnumerable<Route> GetRoutesFromStation(int stationId);
        IEnumerable<Route> GetRoutesBetweenStations(int startStationId, int endStationId);
    }
}