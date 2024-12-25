using PassengerPortal.Server.Data;
using PassengerPortal.Shared.Interfaces;
using PassengerRoute = PassengerPortal.Shared.Models.Route;
using PassengerStation = PassengerPortal.Shared.Models.Station;
using Microsoft.EntityFrameworkCore;


public class RouteRepository : IRouteRepository
{
    private readonly ApplicationDbContext _context;

    public RouteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public PassengerRoute GetById(int id) =>
        _context.Routes
            .Include(r => r.StartStation)
            .Include(r => r.EndStation)
            .Include(r => r.Timetables)
            .FirstOrDefault(r => r.Id == id);

    public IEnumerable<PassengerRoute> GetAll() =>
        _context.Routes
            .Include(r => r.StartStation)
            .Include(r => r.EndStation)
            .Include(r => r.Timetables)
            .ToList();

    public IEnumerable<PassengerRoute> GetRoutesFromStation(int stationId) =>
        _context.Routes
            .Include(r => r.StartStation)
            .Include(r => r.EndStation)
            .Include(r => r.Timetables) // Dodanie Timetables
            .Where(r => r.StartStation.Id == stationId);

    public IEnumerable<PassengerRoute> GetRoutesBetweenStations(int startStationId, int endStationId) =>
        _context.Routes
            .Include(r => r.StartStation)
            .Include(r => r.EndStation)
            .Include(r => r.Timetables) // Dodanie Timetables
            .Where(r => r.StartStation.Id == startStationId && r.EndStation.Id == endStationId);

}