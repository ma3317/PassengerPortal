using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        void Delete(Ticket ticket);
        IEnumerable<Ticket> GetAll();
        Ticket GetById(int id);
        int GetSoldTickets(int routeId, DateTime departureDateTime);
        void Update(Ticket ticket);
        void Save();
    }
}
