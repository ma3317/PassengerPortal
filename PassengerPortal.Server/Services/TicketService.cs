using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PassengerPortal.Server.Data;

namespace PassengerPortal.Server.Services
{
    public class TicketService : ITicketService
    {
        private readonly IRouteRepository _routeRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly ApplicationDbContext _context;

        public TicketService(IRouteRepository routeRepo, ITicketRepository ticketRepo, ApplicationDbContext context)
        {
            _routeRepo = routeRepo;
            _ticketRepo = ticketRepo;
            _context = context;
        }

        public bool PurchaseTicket(List<int> routeIds, DateTime departureDateTime, string buyerId, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Pobierz trasy
                    var routes = _routeRepo.GetAll()
                        .Where(r => routeIds.Contains(r.Id))
                        .ToList();

                    if (routes.Count != routeIds.Count)
                    {
                        errorMessage = "Jedna lub więcej tras nie istnieje.";
                        return false;
                    }

                    // Sprawdź dostępność miejsc na każdej trasie
                    foreach (var route in routes)
                    {
                        var soldTickets = _ticketRepo.GetSoldTickets(route.Id, departureDateTime);
                        if (soldTickets >= route.AvailableSeats)
                        {
                            errorMessage = $"Brak dostępnych miejsc na trasie {route.StartStation.Name} → {route.EndStation.Name}.";
                            return false;
                        }
                    }

                    // Zsumuj ceny
                    decimal totalPrice = routes.Sum(r => r.Price);

                    // Stwórz bilet
                    var ticket = new Ticket
                    {
                        Routes = routes,
                        PurchaseTime = DateTime.UtcNow,
                        Price = totalPrice,
                        BuyerId = buyerId
                    };

                    // Dodaj bilet do repozytorium
                    _ticketRepo.Add(ticket);

                    // Zapisz zmiany
                    _ticketRepo.Save();

                    // Zatwierdź transakcję
                    transaction.Commit();

                    Console.WriteLine($"Bilet kupiony pomyślnie: {buyerId}, trasy: {string.Join(", ", routes.Select(r => $"{r.StartStation.Name} → {r.EndStation.Name}"))}");
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    errorMessage = $"Wystąpił błąd podczas zakupu biletu: {ex.Message}";
                    return false;
                }
            }
        }
    }
}
