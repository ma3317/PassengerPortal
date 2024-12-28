using PassengerPortal.Server.Data;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace PassengerPortal.Server.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _context.Tickets
                .Include(t => t.Routes)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .ToList();
        }

        public Ticket GetById(int id)
        {
            return _context.Tickets
                .Include(t => t.Routes)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .FirstOrDefault(t => t.Id == id);
        }

        public int GetSoldTickets(int routeId, DateTime departureDateTime)
        {
            return _context.Tickets
                .Where(t => t.Routes.Any(r => r.Id == routeId) && t.DepartureDateTime == departureDateTime)
                .Count();
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}