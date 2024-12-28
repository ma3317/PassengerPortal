using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassengerPortal.Shared.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime DepartureDateTime { get; set; }
        // Relacja wiele-do-wielu z Route
        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public Station StartStation { get; set; }
        public int StartStationId { get; set; }
        public Station EndStation { get; set; }
        public int EndStationId { get; set; }
        public DateTime PurchaseTime { get; set; }
        public decimal Price { get; set; }

        [Required]
        public string BuyerId { get; set; } // Zakładając, że masz system użytkowników
    }
}


