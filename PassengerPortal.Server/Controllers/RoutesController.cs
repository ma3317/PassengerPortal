using Microsoft.AspNetCore.Mvc;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System.Collections.Generic;
using Route = PassengerPortal.Shared.Models.Route;
namespace PassengerPortal.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteRepository _routeRepo;

        public RoutesController(IRouteRepository routeRepo)
        {
            _routeRepo = routeRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Route>> GetAll()
        {
            return Ok(_routeRepo.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Route> GetById(int id)
        {
            var route = _routeRepo.GetById(id);
            if (route == null)
                return NotFound();

            return Ok(route);
        }

        [HttpGet("from/{stationId}")]
        public ActionResult<IEnumerable<Route>> GetRoutesFromStation(int stationId)
        {
            var routes = _routeRepo.GetRoutesFromStation(stationId);
            return Ok(routes);
        }

        [HttpGet("between")]
        public ActionResult<IEnumerable<Route>> GetRoutesBetweenStations([FromQuery] int startStationId, [FromQuery] int endStationId)
        {
            var routes = _routeRepo.GetRoutesBetweenStations(startStationId, endStationId);
            return Ok(routes);
        }
    }
}