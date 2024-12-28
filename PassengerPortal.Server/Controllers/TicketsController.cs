using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace PassengerPortal.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("purchase")]
        public IActionResult PurchaseTicket([FromBody] PurchaseTicketRequest request)
        {
            if (_ticketService.PurchaseTicket(request.RouteIds, request.DepartureDateTime, request.BuyerId, out string errorMessage))
            {
                return Ok(new { Message = "Bilet zakupiony pomyślnie." });
            }
            else
            {
                return BadRequest(new { Message = errorMessage });
            }
        }
    }
}


public class PurchaseTicketRequest
{
    public List<int> RouteIds { get; set; }
    public DateTime DepartureDateTime { get; set; }
    public string BuyerId { get; set; }
}
