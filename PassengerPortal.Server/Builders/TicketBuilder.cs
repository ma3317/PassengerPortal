/*using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Builders;

public class TicketBuilder : ITicketBuilder
{
    private Passenger _passenger;
    private Connection _connection;
    private decimal _basePrice;
    private IDiscountStrategy _discountStrategy;
    private DateTime _travelDate;

    public ITicketBuilder SetPassenger(Passenger passenger)
    {
        _passenger = passenger;
        return this;
    }

    public ITicketBuilder SetConnection(Connection connection)
    {
        _connection = connection;
        return this;
    }

    public ITicketBuilder SetBasePrice(decimal price)
    {
        _basePrice = price;
        return this;
    }

    public ITicketBuilder SetDiscountStrategy(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
        return this;
    }

    public ITicketBuilder SetTravelDate(DateTime date)
    {
        _travelDate = date;
        return this;
    }

    public Ticket Build()
    {
        var finalPrice = _discountStrategy?.ApplyDiscount(_basePrice, _passenger, _travelDate) ?? _basePrice;

        return new Ticket
        {
            Passenger = _passenger,
            Connection = _connection,
            BasePrice = _basePrice,
            FinalPrice = finalPrice,
            PurchaseTime = DateTime.Now,
            TravelDate = _travelDate
        };
    }
}*/