namespace PassengerPortal.Shared.Models;

public class Refund
{
    public int Id { get; set; }
    public Ticket Ticket { get; set; }
    public decimal RefundAmount { get; set; }
    public DateTime RefundDate { get; set; }
    public string Reason { get; set; }
    public Passenger RequestedBy { get; set; }
}