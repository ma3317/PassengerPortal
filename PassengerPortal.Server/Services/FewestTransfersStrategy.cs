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

        public IEnumerable<Connection> SearchConnections(Station start, Station end, DateTime departureTime)
        {
            // BFS: chcemy zminimalizować liczbę przesiadek.
            // W kolejce trzymamy stację, bieżący czas, oraz listę dotychczasowych tras (Routes).
            var queue = new Queue<(Station currentStation, DateTime currentTime, List<Route> currentRoute)>();
            var visited = new HashSet<int>();

            // Inicjalizacja
            queue.Enqueue((start, departureTime, new List<Route>()));
            visited.Add(start.Id);

            while (queue.Count > 0)
            {
                var (currentStation, currentTime, currentRoute) = queue.Dequeue();

                // Jeśli dotarliśmy do stacji docelowej, zwracamy połączenie (1 wynik).
                if (currentStation.Id == end.Id)
                {
                    return new List<Connection>
                    {
                        new Connection { Routes = currentRoute }
                    };
                }

                // Pobieramy wszystkie trasy wychodzące ze stacji 'currentStation'.
                var outgoingRoutes = _routeRepo.GetRoutesFromStation(currentStation.Id);

                foreach (var route in outgoingRoutes)
                {
                    // Szukamy najbliższego odjazdu w danym dniu; jeśli nie ma, to przechodzimy do dnia następnego.
                    var nextDeparture = FindNextDeparture(route, currentTime);
                    if (nextDeparture == null)
                        continue; // Brak możliwego odjazdu

                    // Obliczamy potencjalny czas przyjazdu na stację końcową tej trasy
                    var arrivalTime = nextDeparture.Value.Add(route.Duration);
                    var nextStation = route.EndStation;

                    // Aby ograniczyć liczbę odwiedzin tej samej stacji (co upraszcza BFS),
                    // sprawdzamy, czy nie była już odwiedzona. 
                    // Jeżeli nie, to dodajemy do kolejki.
                    if (!visited.Contains(nextStation.Id))
                    {
                        visited.Add(nextStation.Id);

                        // Tworzymy nową listę tras (powiększoną o bieżącą trasę).
                        var newRoute = new List<Route>(currentRoute) { route };

                        // Wrzucamy do kolejki następną stację, nowy bieżący czas i nową ścieżkę tras.
                        queue.Enqueue((nextStation, arrivalTime, newRoute));
                    }
                }
            }

            // Jeśli nie znaleźliśmy żadnej ścieżki do stacji docelowej, zwracamy pustą kolekcję
            return Enumerable.Empty<Connection>();
        }

        /// <summary>
        /// Metoda wyszukująca najbliższy możliwy odjazd w TYM lub NASTĘPNYM dniu,
        /// zakładając, że pociąg kursuje codziennie o tych samych godzinach.
        /// </summary>
        private DateTime? FindNextDeparture(Route route, DateTime currentArrival)
        {
            var timePart = currentArrival.TimeOfDay; // Wyciągamy TimeSpan z bieżącego czasu.

            // Sortujemy Timetables po DepartureTime (który teraz jest TimeSpan).
            var sortedTimetables = route.Timetables
                .OrderBy(t => t.DepartureTime)
                .ToList();

            // KROK 1: sprawdzamy, czy istnieje pociąg "dzisiaj":
            var sameDayTimetable = sortedTimetables
                .FirstOrDefault(t => t.DepartureTime >= timePart);

            if (sameDayTimetable != null)
            {
                // Zwracamy pełny DateTime, łącząc datę z currentArrival oraz DepartureTime jako TimeSpan.
                return currentArrival.Date + sameDayTimetable.DepartureTime;
            }

            // KROK 2: jeśli nie ma dzisiaj, bierzemy pierwszy kurs "jutro":
            var nextDayTimetable = sortedTimetables.FirstOrDefault();
            if (nextDayTimetable != null)
            {
                return currentArrival.Date.AddDays(1) + nextDayTimetable.DepartureTime;
            }

            // Jeśli brak wpisów, zwracamy null.
            return null;
        }

    }
}
