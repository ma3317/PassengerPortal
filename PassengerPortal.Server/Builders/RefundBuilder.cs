/*using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Builders
{
    public class RefundBuilder : IRefundBuilder
    {
        private Refund _refund = new Refund();

        public IRefundBuilder SetTicket(Ticket ticket)
        {
            _refund.Ticket = ticket;
            return this;
        }

        public IRefundBuilder SetRefundAmount(decimal amount)
        {
            _refund.RefundAmount = amount;
            return this;
        }

        public IRefundBuilder SetRefundDate(DateTime date)
        {
            _refund.RefundDate = date;
            return this;
        }

        public IRefundBuilder SetReason(string reason)
        {
            _refund.Reason = reason;
            return this;
        }

        public IRefundBuilder SetRequestedBy(Passenger passenger)
        {
            _refund.RequestedBy = passenger;
            return this;
        }

        public Refund Build()
        {
            // Możesz dodać walidację lub dodatkową logikę tutaj
            return _refund;
        }
    }
}*/