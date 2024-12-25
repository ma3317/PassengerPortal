namespace PassengerPortal.Shared.Models;

public class Ticket
{
    public int Id { get; set; }
    public Passenger Passenger { get; set; }
    public Connection Connection { get; set; }
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime PurchaseTime { get; set; }
    public DateTime TravelDate { get; set; }
    
    protected ITicketRenderer Renderer;

    protected Ticket(ITicketRenderer renderer)
    {
        Renderer = renderer;
    }

    //public abstract void Generate();
}
