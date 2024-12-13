using Microsoft.AspNetCore.Mvc;
using PassengerPortal.Server.Data;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStations()
        {
            var stations = _context.Stations.ToList();
            return Ok(stations);
        }
    }
}