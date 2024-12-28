using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

/*namespace PassengerPortal.Shared.Interfaces
{
    public interface ISearchStrategy
    {
        IEnumerable<Connection> SearchConnections(Station start, Station end, DateTime departureTime);
    }
}*/


// PassengerPortal.Shared.Interfaces.ISearchStrategy.cs
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface ISearchStrategy
    {
        IEnumerable<Connection> SearchConnections(
            Station start,
            Station end,
            DateTime departureTime,
            int maxResults = 5,
            IConnectionFilter filter = null // Opcjonalny filtr
        );}
}


