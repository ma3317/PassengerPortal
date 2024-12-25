namespace PassengerPortal.Shared.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public Passenger Passenger { get; set; }
        public Ticket Ticket { get; set; }
        public Refund Refund { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } // "Purchase", "Refund"
    }
}