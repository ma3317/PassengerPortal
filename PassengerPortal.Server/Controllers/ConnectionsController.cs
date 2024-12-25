using Microsoft.AspNetCore.Mvc;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;
using Route = PassengerPortal.Shared.Models.Route;

namespace PassengerPortal.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly IStationRepository _stationRepo;
        private readonly ISearchStrategy _searchStrategy;

        public ConnectionsController(IStationRepository stationRepo, ISearchStrategy searchStrategy)
        {
            _stationRepo = stationRepo;
            _searchStrategy = searchStrategy;
        }

        /*[HttpGet("search")]
        public ActionResult<IEnumerable<Connection>> SearchConnections([FromQuery] int startStationId, [FromQuery] int endStationId, [FromQuery] DateTime departureTime)
        {
            // Logowanie wejściowych danych
            Console.WriteLine($"Rozpoczęto wyszukiwanie połączenia: startStationId={startStationId}, endStationId={endStationId}, departureTime={departureTime}");

            departureTime = departureTime.ToUniversalTime();
            var startStation = _stationRepo.GetById(startStationId);
            var endStation = _stationRepo.GetById(endStationId);

            if (startStation == null || endStation == null)
            {
                Console.WriteLine("Nie znaleziono stacji początkowej lub końcowej.");
                return BadRequest("Invalid station IDs.");
            }

            // Logowanie przed wyszukiwaniem
            Console.WriteLine($"Wyszukiwanie połączeń: start={startStation.Name}, end={endStation.Name}, departureTime={departureTime}");

            var connections = _searchStrategy.SearchConnections(startStation, endStation, departureTime);

            // Logowanie wyników
            if (connections == null || !connections.Any())
            {
                Console.WriteLine("Nie znaleziono żadnych połączeń.");
                return Ok(new List<Connection>()); // Zwraca pustą listę
            }

            Console.WriteLine($"Znaleziono {connections.Count()} połączeń.");
            return Ok(connections);
        }*/
        [HttpGet("search")]
    public ActionResult<IEnumerable<Connection>> SearchConnections([FromQuery] int startStationId, [FromQuery] int endStationId, [FromQuery] DateTime departureTime)
    {
        // Ustawienie czasu na UTC, jeśli potrzebne
        departureTime = departureTime.ToUniversalTime();
        var startStation = _stationRepo.GetById(startStationId);
        var endStation = _stationRepo.GetById(endStationId);

        if (startStation == null || endStation == null)
            return BadRequest("Invalid station IDs.");

        Console.WriteLine($"Rozpoczęto wyszukiwanie połączenia: startStationId={startStationId}, endStationId={endStationId}, departureTime={departureTime}");

        var connections = _searchStrategy.SearchConnections(startStation, endStation, departureTime);

        if (connections == null || !connections.Any())
        {
            Console.WriteLine("Nie znaleziono żadnych połączeń.");
            return Ok(new List<Connection>());
        }

        // Przekształcenie połączeń do odpowiedzi JSON
        var connectionDtos = connections.Select(c => new Connection
        {
            Routes = c.Routes.Select(r => new Route
            {
                Id = r.Id,
                StartStation = new Station
                {
                    Id = r.StartStation.Id,
                    Name = r.StartStation.Name,
                    City = r.StartStation.City
                },
                EndStation = new Station
                {
                    Id = r.EndStation.Id,
                    Name = r.EndStation.Name,
                    City = r.EndStation.City
                },
                Duration = r.Duration,
                DepartureDateTime = r.DepartureDateTime,
                ArrivalDateTime = r.ArrivalDateTime,
                Timetables = r.Timetables.Select(t => new Timetable
                {
                    Id = t.Id,
                    RouteId = t.RouteId,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime
                }).ToList()
            }).ToList()
        }).ToList();

        return Ok(connectionDtos);
    }

    }
}