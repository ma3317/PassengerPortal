using Microsoft.AspNetCore.Mvc;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

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

        [HttpGet("search")]
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
        }

    }
}