using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface ITicketService
    {
        bool PurchaseTicket(List<int> routeIds, DateTime departureDateTime, string buyerId, out string errorMessage);
    }
}
