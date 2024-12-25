using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface ISearchStrategy
    {
        IEnumerable<Connection> SearchConnections(Station start, Station end, DateTime departureTime);
    }
}