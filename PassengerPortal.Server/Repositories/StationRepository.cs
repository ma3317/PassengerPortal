using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using PassengerPortal.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace PassengerPortal.Server.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly ApplicationDbContext _context;

        public StationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Station GetById(int id) => _context.Stations.Find(id);
        public IEnumerable<Station> GetAll() => _context.Stations.ToList();
        public Station GetByName(string name) => _context.Stations.FirstOrDefault(s => s.Name == name);
    }
}