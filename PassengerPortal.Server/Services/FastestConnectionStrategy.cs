using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Route = PassengerPortal.Shared.Models.Route;
//Jasło → Gorlice
// Gorlice → Tarnów
// Tarnów → Kraków tak dodajemy trase Jasło-Gorlice-Tarnów-Kraków
//strategia wyszukuje jedynie 1 najbardziej pasujące połączenie 
namespace PassengerPortal.Server.Services
{
    public class FastestConnectionStrategy : ISearchStrategy
    {
        private readonly IRouteRepository _routeRepo;
        private readonly IStationRepository _stationRepo;

        // Konstruktor inicjalizujący strategię. Otrzymuje repozytoria tras i stacji.
        public FastestConnectionStrategy(IRouteRepository routeRepo, IStationRepository stationRepo)
        {
            _routeRepo = routeRepo;
            _stationRepo = stationRepo;
        }

        // Metoda wyszukująca najszybsze połączenie między stacjami `start` i `end`, rozpoczynające się po `departureTime`.
        public IEnumerable<Connection> SearchConnections(Station start, Station end, DateTime departureTime,int maxResults = 1)
        {
            Console.WriteLine($"Start wyszukiwania: start={start.Name}, end={end.Name}, departureTime={departureTime}");

            // Słownik przechowujący najwcześniejsze znane czasy przybycia na każdą stację.
            var earliestArrival = new Dictionary<int, DateTime>();

            // Słowniki do przechowywania poprzednich stacji i tras w ścieżce do aktualnej stacji.
            var previousStation = new Dictionary<int, int>();
            var previousRoute = new Dictionary<int, Route>();

            // Inicjalizacja czasów przybycia jako maksymalne wartości.
            foreach (var station in _stationRepo.GetAll())
            {
                earliestArrival[station.Id] = DateTime.MaxValue;
                Console.WriteLine($"Inicjalizacja: Stacja={station.Name}, earliestArrival={earliestArrival[station.Id]}");
            }

            // Ustawienie czasu przybycia do stacji początkowej jako `departureTime`.
            earliestArrival[start.Id] = departureTime;

            // Kolejka priorytetowa do przetwarzania stacji według najszybszego czasu przybycia.
            var pq = new PriorityQueue<(int stationId, DateTime arrTime), DateTime>();
            pq.Enqueue((start.Id, departureTime), departureTime);

            while (pq.Count > 0)
            {
                // Pobranie stacji z najwcześniejszym czasem przybycia.
                var (currentStationId, currentArrival) = pq.Dequeue();
                Console.WriteLine($"Rozpatrywana stacja: {currentStationId}, czas przybycia: {currentArrival}");

                // Jeśli dotarliśmy do stacji docelowej, rekonstruujemy ścieżkę i zwracamy wynik.
                if (currentStationId == end.Id)
                {
                    Console.WriteLine("Znaleziono stację docelową!");
                    //var connection = ReconstructConnection(previousStation, previousRoute, start.Id, end.Id);
                    var connection = ReconstructConnection(previousStation, previousRoute, earliestArrival, start.Id, end.Id);

                    return new List<Connection> { connection };
                }

                // Ignorowanie stacji, jeśli znaleziono szybszy sposób dotarcia do niej.
                if (currentArrival > earliestArrival[currentStationId])
                {
                    Console.WriteLine($"Pomijam stację {currentStationId}, ponieważ currentArrival > earliestArrival.");
                    continue;
                }

                // Pobieranie wszystkich wychodzących tras z aktualnej stacji.
                var outgoingRoutes = _routeRepo.GetRoutesFromStation(currentStationId);
                Console.WriteLine($"Stacja {currentStationId} ma {outgoingRoutes.Count()} wychodzących tras.");

                foreach (var route in outgoingRoutes)
                {
                    Console.WriteLine($"Sprawdzanie trasy: {route.StartStation.Name} -> {route.EndStation.Name}");
                    
                    // Znajdowanie najbliższego odjazdu na trasie.
                    var candidateDeparture = FindNextDeparture(route, currentArrival);

                    if (candidateDeparture == null)
                    {
                        Console.WriteLine("Brak kolejnych odjazdów na tej trasie.");
                        continue;
                    }

                    // Obliczenie potencjalnego czasu przybycia na końcową stację trasy.
                    DateTime potentialArrival = candidateDeparture.Value.Add(route.Duration);
                    Console.WriteLine($"Potencjalny czas przybycia na stację {route.EndStation.Name}: {potentialArrival}");

                    int endStationId = route.EndStation.Id;

                    // Aktualizacja najwcześniejszego czasu przybycia i tras, jeśli znaleziono szybszą drogę.
                    if (potentialArrival < earliestArrival[endStationId])
                    {
                        earliestArrival[endStationId] = potentialArrival;
                        previousStation[endStationId] = currentStationId;
                        previousRoute[endStationId] = route;
                        pq.Enqueue((endStationId, potentialArrival), potentialArrival);

                        Console.WriteLine($"Zaktualizowano earliestArrival dla stacji {endStationId} na {potentialArrival}");
                    }
                }
            }

            Console.WriteLine("Nie znaleziono żadnego połączenia.");
            return Enumerable.Empty<Connection>();
        }

        // Znajduje najbliższy odjazd na danej trasie, zaczynając od `currentArrival`.
        private DateTime? FindNextDeparture(Route route, DateTime currentArrival)
        {
            TimeSpan currentTimeOfDay = currentArrival.TimeOfDay;
            Console.WriteLine($"Sprawdzanie odjazdów na trasie {route.StartStation.Name} -> {route.EndStation.Name}, po czasie {currentTimeOfDay}");

            // Pobranie rozkładu jazdy posortowanego według godzin odjazdu.
            var sortedTimetables = route.Timetables
                .OrderBy(t => t.DepartureTime)
                .ToList();

            foreach (var timetable in sortedTimetables)
            {
                Console.WriteLine($"Rozkład jazdy: Odjazd={timetable.DepartureTime}, Przyjazd={timetable.ArrivalTime}");
            }

            // Znajdowanie pierwszego odjazdu "dzisiaj" po podanym czasie.
            var sameDayTimetable = sortedTimetables
                .FirstOrDefault(t => t.DepartureTime >= currentTimeOfDay);

            if (sameDayTimetable != null)
            {
                DateTime departureToday = currentArrival.Date + sameDayTimetable.DepartureTime;
                Console.WriteLine($"Najbliższy odjazd dzisiaj: {departureToday}");
                return departureToday;
            }

            // Znajdowanie pierwszego odjazdu "jutro", jeśli brak odjazdów "dzisiaj".
            var nextDayTimetable = sortedTimetables.FirstOrDefault();
            if (nextDayTimetable != null)
            {
                DateTime departureTomorrow = currentArrival.Date.AddDays(1) + nextDayTimetable.DepartureTime;
                Console.WriteLine($"Najbliższy odjazd jutro: {departureTomorrow}");
                return departureTomorrow;
            }

            Console.WriteLine("Brak odjazdów na tej trasie.");
            return null;
        }
        private Connection ReconstructConnection(Dictionary<int, int> previousStation, Dictionary<int, Route> previousRoute, Dictionary<int, DateTime> earliestArrival, int startId, int endId)
        {
            Console.WriteLine("Rozpoczynam rekonstrukcję połączenia...");
            List<Route> routes = new List<Route>();
            int current = endId;
            DateTime arrivalTime = earliestArrival[endId];

            while (current != startId)
            {
                if (!previousStation.ContainsKey(current))
                {
                    Console.WriteLine($"Nie można znaleźć poprzedniej stacji dla {current}");
                    break;
                }

                var route = previousRoute[current];
                var previousStationId = previousStation[current];
                DateTime departureTime = arrivalTime - route.Duration;

                // Ustawienie DepartureDateTime i ArrivalDateTime
                route.DepartureDateTime = departureTime;
                route.ArrivalDateTime = arrivalTime;

                routes.Add(route);
                arrivalTime = departureTime;
                current = previousStationId;
            }

            routes.Reverse();
            Console.WriteLine("Rekonstrukcja zakończona.");
            return new Connection { Routes = routes };
        }


    }
}