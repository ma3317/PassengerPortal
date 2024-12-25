namespace PassengerPortal.Shared.Models;

public class StandardTicket : Ticket
{
    public StandardTicket(ITicketRenderer renderer) : base(renderer) { }

    /*public override void Generate()
    {
        Renderer.Render(this);
    }*/
}