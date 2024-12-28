/*using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using Route = PassengerPortal.Shared.Models.Route;
// Wyszukuje 5 najbliższych połączeń w tym w kolejnych dniach 


namespace PassengerPortal.Server.Services
{
    public class FewestConnectionStrategy : ISearchStrategy
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IStationRepository _stationRepo;

        public FewestConnectionStrategy(IRouteRepository routeRepo, IStationRepository stationRepo)
        {
            _routeRepo = routeRepo;
            _stationRepo = stationRepo;
        }

        /// <summary>
        /// Wyszukuje do maxResults (domyślnie 5) najszybszych połączeń (z przesiadkami),
        /// zaczynając nie wcześniej niż departureTime.
        /// </summary>
        public IEnumerable<Connection> SearchConnections(
            Station start,
            Station end,
            DateTime departureTime,
            int maxResults = 5)
        {
            Console.WriteLine($"[SearchConnections] start={start.Name}, end={end.Name}, departureTime={departureTime}, maxResults={maxResults}");

            var foundConnections = new List<Connection>();
            DateTime currentDepartureTime = departureTime;

            // Pętla, aby znaleźć do maxResults połączeń
            while (foundConnections.Count < maxResults)
            {
                var connection = FindConnection(start, end, currentDepartureTime);

                if (connection == null)
                {
                    Console.WriteLine("[SearchConnections] Nie znaleziono więcej połączeń.");
                    break;
                }

                // Sprawdzenie unikalności połączenia na podstawie najwcześniejszego odjazdu
                var earliestDeparture = connection.Routes.Min(r => r.DepartureDateTime);
                bool alreadyExists = foundConnections.Any(c =>
                    c.Routes.Min(rr => rr.DepartureDateTime) == earliestDeparture);

                if (!alreadyExists)
                {
                    foundConnections.Add(connection);
                    Console.WriteLine($"[SearchConnections] Dodano połączenie z {earliestDeparture}");

                    // Aktualizacja departureTime, aby znaleźć kolejne połączenie
                    currentDepartureTime = earliestDeparture.AddMinutes(1);
                }
                else
                {
                    Console.WriteLine($"[SearchConnections] Połączenie z {earliestDeparture} już istnieje, pomijam.");
                    // Jeśli połączenie jest duplikatem, zwiększamy departureTime
                    currentDepartureTime = earliestDeparture.AddMinutes(1);
                }
            }

            // Sortowanie znalezionych połączeń według najwcześniejszego odjazdu
            var sortedConnections = foundConnections
                .OrderBy(c => c.Routes.Min(r => r.DepartureDateTime))
                .Take(maxResults)
                .ToList();

            Console.WriteLine($"[SearchConnections] Zwracam {sortedConnections.Count} połączeń.");
            return sortedConnections;
        }

        /// <summary>
        /// Metoda BFS/Dijkstra, która znajduje jedno najwcześniejsze połączenie (z przesiadkami) 
        /// od stacji 'start' do 'end', zaczynając w 'departureTime'.
        /// </summary>
        private Connection? FindConnection(Station start, Station end, DateTime departureTime)
        {
            Console.WriteLine($"[FindConnection] start={start.Name}, end={end.Name}, departureTime={departureTime}");

            var earliestArrival = new Dictionary<int, DateTime>();
            var previousStation = new Dictionary<int, int>();
            var previousRoute = new Dictionary<int, Route>();

            // Inicjalizacja czasów przybycia
            foreach (var station in _stationRepo.GetAll())
            {
                earliestArrival[station.Id] = DateTime.MaxValue;
                Console.WriteLine($"[FindConnection] Inicjalizacja: Stacja={station.Name}, earliestArrival={earliestArrival[station.Id]}");
            }

            earliestArrival[start.Id] = departureTime;

            // Kolejka priorytetowa (klucz: czas przybycia)
            var pq = new PriorityQueue<(int stationId, DateTime arrivalTime), DateTime>();
            pq.Enqueue((start.Id, departureTime), departureTime);

            while (pq.Count > 0)
            {
                var (currentStationId, currentArrival) = pq.Dequeue();
                Console.WriteLine($"[FindConnection] Rozpatrywana stacja: {currentStationId}, czas przybycia: {currentArrival}");

                // Jeśli dotarliśmy do stacji docelowej, rekonstruujemy połączenie
                if (currentStationId == end.Id)
                {
                    Console.WriteLine("[FindConnection] Dotarliśmy do stacji docelowej, rekonstruowanie połączenia...");
                    var connection = ReconstructConnection(previousStation, previousRoute, earliestArrival, start.Id, end.Id);
                    return connection;
                }

                // Ignorowanie stacji, jeśli znaleziono szybszy sposób dotarcia do niej
                if (currentArrival > earliestArrival[currentStationId])
                {
                    Console.WriteLine($"[FindConnection] Pomijam stację {currentStationId}, ponieważ currentArrival > earliestArrival.");
                    continue;
                }

                // Pobieranie wszystkich wychodzących tras z aktualnej stacji
                var outgoingRoutes = _routeRepo.GetRoutesFromStation(currentStationId);
                Console.WriteLine($"[FindConnection] Stacja {currentStationId} ma {outgoingRoutes.Count()} wychodzących tras.");

                foreach (var route in outgoingRoutes)
                {
                    Console.WriteLine($"[FindConnection] Sprawdzanie trasy: {route.StartStation.Name} -> {route.EndStation.Name}");

                    // Znajdowanie najbliższego odjazdu na trasie po czasie currentArrival
                    var candidateDeparture = FindNextDeparture(route, currentArrival);

                    if (candidateDeparture == null)
                    {
                        Console.WriteLine("[FindConnection] Brak kolejnych odjazdów na tej trasie.");
                        continue;
                    }

                    // Obliczenie potencjalnego czasu przybycia na końcową stację trasy
                    DateTime potentialArrival = candidateDeparture.Value.Add(route.Duration);
                    Console.WriteLine($"[FindConnection] Potencjalny czas przybycia na stację {route.EndStation.Name}: {potentialArrival}");

                    int endStationId = route.EndStation.Id;

                    // Aktualizacja najwcześniejszego czasu przybycia i tras, jeśli znaleziono szybszą drogę
                    if (potentialArrival < earliestArrival[endStationId])
                    {
                        earliestArrival[endStationId] = potentialArrival;
                        previousStation[endStationId] = currentStationId;
                        previousRoute[endStationId] = route;
                        pq.Enqueue((endStationId, potentialArrival), potentialArrival);

                        Console.WriteLine($"[FindConnection] Zaktualizowano earliestArrival dla stacji {endStationId} na {potentialArrival}");
                    }
                }
            }

            Console.WriteLine("[FindConnection] Nie znaleziono żadnego połączenia.");
            return null;
        }

        /// <summary>
        /// Znajduje najbliższy odjazd (DateTime) na danej trasie 'route', 
        /// biorąc pod uwagę 'currentArrival' (kiedy docieramy do stacji startowej danej trasy).
        /// </summary>
        private DateTime? FindNextDeparture(Route route, DateTime currentArrival)
        {
            Console.WriteLine($"[FindNextDeparture] route={route.StartStation.Name}->{route.EndStation.Name}, currentArrival={currentArrival}");
            var currentTimeOfDay = currentArrival.TimeOfDay;

            // Sortujemy timetables wg DepartureTime
            var sortedTimetables = route.Timetables
                .OrderBy(t => t.DepartureTime)
                .ToList();

            // 1) Znajdź najbliższy odjazd "dzisiaj" (czyli currentArrival.Date) >= currentTimeOfDay
            var sameDay = sortedTimetables.FirstOrDefault(t => t.DepartureTime >= currentTimeOfDay);
            if (sameDay != null)
            {
                var departureToday = currentArrival.Date + sameDay.DepartureTime;
                if (departureToday >= currentArrival)
                {
                    Console.WriteLine($"[FindNextDeparture] Najbliższy odjazd dzisiaj: {departureToday}");
                    return departureToday;
                }
            }

            // 2) Jeśli brak "dzisiaj", bierzemy pierwszy odjazd "jutro"
            var firstTomorrow = sortedTimetables.FirstOrDefault();
            if (firstTomorrow != null)
            {
                var departureTomorrow = currentArrival.Date.AddDays(1) + firstTomorrow.DepartureTime;
                Console.WriteLine($"[FindNextDeparture] Najbliższy odjazd jutro: {departureTomorrow}");
                return departureTomorrow;
            }

            Console.WriteLine("[FindNextDeparture] Brak odjazdów na tej trasie.");
            return null;
        }

        /// <summary>
        /// Rekonstruuje pełne (możliwe wieloodcinkowe) połączenie na podstawie BFS-owych struktur:
        /// previousStation i previousRoute.
        /// </summary>
        private Connection ReconstructConnection(
            Dictionary<int, int> previousStation,
            Dictionary<int, Route> previousRoute,
            Dictionary<int, DateTime> earliestArrival,
            int startId,
            int endId)
        {
            Console.WriteLine("[ReconstructConnection] Rozpoczynam rekonstrukcję połączenia...");
            var routes = new List<Route>();
            int current = endId;
            DateTime arrivalTime = earliestArrival[endId];

            while (current != startId)
            {
                if (!previousStation.ContainsKey(current))
                {
                    Console.WriteLine($"[ReconstructConnection] Brak poprzedniej stacji dla {current}, przerwanie rekonstrukcji.");
                    break;
                }

                var usedRoute = previousRoute[current];
                int prevStationId = previousStation[current];
                DateTime departureTime = arrivalTime - usedRoute.Duration;

                // Tworzymy kopię route, aby nie nadpisywać oryginalnego obiektu
                var newRoute = new Route
                {
                    Id = usedRoute.Id,
                    StartStation = usedRoute.StartStation,
                    EndStation = usedRoute.EndStation,
                    Duration = usedRoute.Duration,
                    DepartureDateTime = departureTime,
                    ArrivalDateTime = arrivalTime,
                    Timetables = usedRoute.Timetables.Select(tt => new Timetable
                    {
                        Id = tt.Id,
                        RouteId = tt.RouteId,
                        DepartureTime = tt.DepartureTime,
                        ArrivalTime = tt.ArrivalTime
                    }).ToList()
                };

                routes.Add(newRoute);
                arrivalTime = departureTime;
                current = prevStationId;
            }

            routes.Reverse();
            Console.WriteLine($"[ReconstructConnection] Rekonstrukcja zakończona, liczba tras w połączeniu: {routes.Count}");

            return new Connection
            {
                Routes = routes
            };
        }
    }
}*/
// PassengerPortal.Server.Services.FewestConnectionStrategy.cs
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Route = PassengerPortal.Shared.Models.Route;

namespace PassengerPortal.Server.Services
{
    public class FewestTransfersStrategy : ISearchStrategy
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IStationRepository _stationRepo;

        public FewestTransfersStrategy(IRouteRepository routeRepo, IStationRepository stationRepo)
        {
            _routeRepo = routeRepo;
            _stationRepo = stationRepo;
        }

        /// <summary>
        /// Wyszukuje do maxResults połączeń z przesiadkami,
        /// zaczynając nie wcześniej niż departureTime, i stosuje filtry.
        /// </summary>
        public IEnumerable<Connection> SearchConnections(
            Station start,
            Station end,
            DateTime departureTime,
            int maxResults = 5,
            IConnectionFilter? filter = null)
        {
            Console.WriteLine($"[SearchConnections] start={start.Name}, end={end.Name}, departureTime={departureTime}, maxResults={maxResults}");

            var foundConnections = new List<Connection>();
            DateTime currentDepartureTime = departureTime;

            // Pętla, aby znaleźć do maxResults połączeń
            while (foundConnections.Count < maxResults)
            {
                var connection = FindConnection(start, end, currentDepartureTime);

                if (connection == null)
                {
                    Console.WriteLine("[SearchConnections] Nie znaleziono więcej połączeń.");
                    break;
                }

                // Sprawdzenie unikalności połączenia na podstawie najwcześniejszego odjazdu
                var earliestDeparture = connection.Routes.Min(r => r.DepartureDateTime);
                bool alreadyExists = foundConnections.Any(c =>
                    c.Routes.Min(rr => rr.DepartureDateTime) == earliestDeparture);

                if (!alreadyExists)
                {
                    // Opcjonalne filtrowanie
                    if (filter != null)
                    {
                        connection = filter.Apply(new List<Connection> { connection }).FirstOrDefault();
                        if (connection == null)
                        {
                            Console.WriteLine($"[SearchConnections] Połączenie z {earliestDeparture} nie spełnia kryteriów filtra.");
                            currentDepartureTime = earliestDeparture.AddMinutes(1);
                            continue;
                        }
                    }

                    foundConnections.Add(connection);
                    Console.WriteLine($"[SearchConnections] Dodano połączenie z {earliestDeparture}");

                    // Aktualizacja departureTime, aby znaleźć kolejne połączenie
                    currentDepartureTime = earliestDeparture.AddMinutes(1);
                }
                else
                {
                    Console.WriteLine($"[SearchConnections] Połączenie z {earliestDeparture} już istnieje, pomijam.");
                    // Jeśli połączenie jest duplikatem, zwiększamy departureTime
                    currentDepartureTime = earliestDeparture.AddMinutes(1);
                }
            }

            // Sortowanie znalezionych połączeń według najwcześniejszego odjazdu
            var sortedConnections = foundConnections
                .OrderBy(c => c.Routes.Min(r => r.DepartureDateTime))
                .Take(maxResults)
                .ToList();

            Console.WriteLine($"[SearchConnections] Zwracam {sortedConnections.Count} połączeń.");
            return sortedConnections;
        }

        /// <summary>
        /// Metoda BFS/Dijkstra, która znajduje jedno najwcześniejsze połączenie (z przesiadkami) 
        /// od stacji 'start' do 'end', zaczynając w 'departureTime'.
        /// </summary>
        private Connection? FindConnection(Station start, Station end, DateTime departureTime)
        {
            Console.WriteLine($"[FindConnection] start={start.Name}, end={end.Name}, departureTime={departureTime}");

            var earliestArrival = new Dictionary<int, DateTime>();
            var previousStation = new Dictionary<int, int>();
            var previousRoute = new Dictionary<int, Route>();

            // Inicjalizacja czasów przybycia
            foreach (var station in _stationRepo.GetAll())
            {
                earliestArrival[station.Id] = DateTime.MaxValue;
                Console.WriteLine($"[FindConnection] Inicjalizacja: Stacja={station.Name}, earliestArrival={earliestArrival[station.Id]}");
            }

            earliestArrival[start.Id] = departureTime;

            // Kolejka priorytetowa (klucz: czas przybycia)
            var pq = new PriorityQueue<(int stationId, DateTime arrivalTime), DateTime>();
            pq.Enqueue((start.Id, departureTime), departureTime);

            while (pq.Count > 0)
            {
                var (currentStationId, currentArrival) = pq.Dequeue();
                Console.WriteLine($"[FindConnection] Rozpatrywana stacja: {currentStationId}, czas przybycia: {currentArrival}");

                // Jeśli dotarliśmy do stacji docelowej, rekonstruujemy połączenie
                if (currentStationId == end.Id)
                {
                    Console.WriteLine("[FindConnection] Dotarliśmy do stacji docelowej, rekonstruowanie połączenia...");
                    var connection = ReconstructConnection(previousStation, previousRoute, earliestArrival, start.Id, end.Id);
                    return connection;
                }

                // Ignorowanie stacji, jeśli znaleziono szybszy sposób dotarcia do niej
                if (currentArrival > earliestArrival[currentStationId])
                {
                    Console.WriteLine($"[FindConnection] Pomijam stację {currentStationId}, ponieważ currentArrival > earliestArrival.");
                    continue;
                }

                // Pobieranie wszystkich wychodzących tras z aktualnej stacji
                var outgoingRoutes = _routeRepo.GetRoutesFromStation(currentStationId);
                Console.WriteLine($"[FindConnection] Stacja {currentStationId} ma {outgoingRoutes.Count()} wychodzących tras.");

                foreach (var route in outgoingRoutes)
                {
                    Console.WriteLine($"[FindConnection] Sprawdzanie trasy: {route.StartStation.Name} -> {route.EndStation.Name}");

                    // Znajdowanie najbliższego odjazdu na trasie po czasie currentArrival
                    var candidateDeparture = FindNextDeparture(route, currentArrival);

                    if (candidateDeparture == null)
                    {
                        Console.WriteLine("[FindConnection] Brak kolejnych odjazdów na tej trasie.");
                        continue;
                    }

                    // Obliczenie potencjalnego czasu przybycia na końcową stację trasy
                    DateTime potentialArrival = candidateDeparture.Value.Add(route.Duration);
                    Console.WriteLine($"[FindConnection] Potencjalny czas przybycia na stację {route.EndStation.Name}: {potentialArrival}");

                    int endStationId = route.EndStation.Id;

                    // Aktualizacja najwcześniejszego czasu przybycia i tras, jeśli znaleziono szybszą drogę
                    if (potentialArrival < earliestArrival[endStationId])
                    {
                        earliestArrival[endStationId] = potentialArrival;
                        previousStation[endStationId] = currentStationId;
                        previousRoute[endStationId] = route;
                        pq.Enqueue((endStationId, potentialArrival), potentialArrival);

                        Console.WriteLine($"[FindConnection] Zaktualizowano earliestArrival dla stacji {endStationId} na {potentialArrival}");
                    }
                }
            }

            Console.WriteLine("[FindConnection] Nie znaleziono żadnego połączenia.");
            return null;
        }

        /// <summary>
        /// Znajduje najbliższy odjazd (DateTime) na danej trasie 'route', 
        /// biorąc pod uwagę 'currentArrival' (kiedy docieramy do stacji startowej danej trasy).
        /// </summary>
        private DateTime? FindNextDeparture(Route route, DateTime currentArrival)
        {
            Console.WriteLine($"[FindNextDeparture] route={route.StartStation.Name}->{route.EndStation.Name}, currentArrival={currentArrival}");
            var currentTimeOfDay = currentArrival.TimeOfDay;

            // Sortujemy timetables wg DepartureTime
            var sortedTimetables = route.Timetables
                .OrderBy(t => t.DepartureTime)
                .ToList();

            // 1) Znajdź najbliższy odjazd "dzisiaj" (czyli currentArrival.Date) >= currentTimeOfDay
            var sameDay = sortedTimetables.FirstOrDefault(t => t.DepartureTime >= currentTimeOfDay);
            if (sameDay != null)
            {
                var departureToday = currentArrival.Date + sameDay.DepartureTime;
                if (departureToday >= currentArrival)
                {
                    Console.WriteLine($"[FindNextDeparture] Najbliższy odjazd dzisiaj: {departureToday}");
                    return departureToday;
                }
            }

            // 2) Jeśli brak "dzisiaj", bierzemy pierwszy odjazd "jutro"
            var firstTomorrow = sortedTimetables.FirstOrDefault();
            if (firstTomorrow != null)
            {
                var departureTomorrow = currentArrival.Date.AddDays(1) + firstTomorrow.DepartureTime;
                Console.WriteLine($"[FindNextDeparture] Najbliższy odjazd jutro: {departureTomorrow}");
                return departureTomorrow;
            }

            Console.WriteLine("[FindNextDeparture] Brak odjazdów na tej trasie.");
            return null;
        }

        /// <summary>
        /// Rekonstruuje pełne (możliwe wieloodcinkowe) połączenie na podstawie BFS-owych struktur:
        /// previousStation i previousRoute.
        /// </summary>
        private Connection ReconstructConnection(
            Dictionary<int, int> previousStation,
            Dictionary<int, Route> previousRoute,
            Dictionary<int, DateTime> earliestArrival,
            int startId,
            int endId)
        {
            Console.WriteLine("[ReconstructConnection] Rozpoczynam rekonstrukcję połączenia...");
            var routes = new List<Route>();
            int current = endId;
            DateTime arrivalTime = earliestArrival[endId];

            while (current != startId)
            {
                if (!previousStation.ContainsKey(current))
                {
                    Console.WriteLine($"[ReconstructConnection] Brak poprzedniej stacji dla {current}, przerwanie rekonstrukcji.");
                    break;
                }

                var usedRoute = previousRoute[current];
                int prevStationId = previousStation[current];
                DateTime departureTime = arrivalTime - usedRoute.Duration;

                // Tworzymy kopię route, aby nie nadpisywać oryginalnego obiektu
                var newRoute = new Route
                {
                    Id = usedRoute.Id,
                    StartStation = usedRoute.StartStation,
                    EndStation = usedRoute.EndStation,
                    Duration = usedRoute.Duration,
                    DepartureDateTime = departureTime,
                    ArrivalDateTime = arrivalTime,
                    Timetables = usedRoute.Timetables.Select(tt => new Timetable
                    {
                        Id = tt.Id,
                        RouteId = tt.RouteId,
                        DepartureTime = tt.DepartureTime,
                        ArrivalTime = tt.ArrivalTime
                    }).ToList(),

                    // **Dodanie brakujących właściwości**
                    TrainType = usedRoute.TrainType,
                    Price = usedRoute.Price,
                    AvailableSeats = usedRoute.AvailableSeats
                };

                routes.Add(newRoute);
                arrivalTime = departureTime;
                current = prevStationId;
            }

            routes.Reverse();
            Console.WriteLine($"[ReconstructConnection] Rekonstrukcja zakończona, liczba tras w połączeniu: {routes.Count}");

            return new Connection
            {
                Routes = routes
            };
        }

    }
}

