using Microsoft.AspNetCore.Mvc;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System.Collections.Generic;

namespace PassengerPortal.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly IStationRepository _stationRepo;

        public StationsController(IStationRepository stationRepo)
        {
            _stationRepo = stationRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Station>> GetAll()
        {
            return Ok(_stationRepo.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Station> GetById(int id)
        {
            var station = _stationRepo.GetById(id);
            if (station == null)
                return NotFound();

            return Ok(station);
        }

        [HttpGet("byname/{name}")]
        public ActionResult<Station> GetByName(string name)
        {
            var station = _stationRepo.GetByName(name);
            if (station == null)
                return NotFound();

            return Ok(station);
        }
    }
}