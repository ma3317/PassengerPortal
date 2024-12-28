/*
using System.Net.Http.Json;
using PassengerPortal.Shared.Models; // Zakładam, że tam masz Connection, Station, itp.

using System.Net.Http.Json;
using PassengerPortal.Shared.Models; // Zakładam, że tam masz Connection, Station, itp.
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PassengerPortal.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Station>> GetStationsAsync()
        {
            var response = await _httpClient.GetAsync("api/stations");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Station>>();
        }

        public async Task<List<Connection>> SearchConnectionsAsync(
            int startStationId,
            int endStationId,
            DateTime departureTime,
            int maxResults = 5) // Domyślnie 5
        {
            // Formatowanie daty w ISO 8601
            var departureTimeFormatted = departureTime.ToUniversalTime().ToString("o");

            // Tworzenie zapytania z parametrami
            var query = $"api/connections/search?startStationId={startStationId}&endStationId={endStationId}" +
                        $"&departureTime={departureTimeFormatted}&maxResults={maxResults}";

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Connection>>();
        }
    }
}*/

// PassengerPortal.Client.Services.ApiService.cs
using System.Net.Http.Json;
using PassengerPortal.Shared.Models; // Zakładam, że tam masz Connection, Station, itp.
using System.Threading.Tasks;
using System.Collections.Generic;
using PassengerPortal.Client.Pages;

namespace PassengerPortal.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Station>> GetStationsAsync()
        {
            var response = await _httpClient.GetAsync("api/stations");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Station>>();
        }

        public async Task<List<Connection>> SearchConnectionsAsync(
            int startStationId,
            int endStationId,
            DateTime departureTime,
            int maxResults = 5, // Domyślnie 5
            decimal? maximumPrice = null,
            TrainType? trainType = null,
            int? minimumSeats = null)
        {
            // Formatowanie daty w ISO 8601
            var departureTimeFormatted = departureTime.ToUniversalTime().ToString("o");

            // Tworzenie listy parametrów
            var queryParams = new List<string>
            {
                $"startStationId={startStationId}",
                $"endStationId={endStationId}",
                $"departureTime={departureTimeFormatted}",
                $"maxResults={maxResults}"
            };

            if (maximumPrice.HasValue)
            {
                queryParams.Add($"maximumPrice={maximumPrice.Value}");
            }

            if (trainType.HasValue)
            {
                queryParams.Add($"trainType={trainType.Value}");
            }

            if (minimumSeats.HasValue)
            {
                queryParams.Add($"minimumSeats={minimumSeats.Value}");
            }

            var query = $"api/connections/search?{string.Join("&", queryParams)}";

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Connection>>();
        }

        public async Task<HttpResponseMessage> PurchaseTicketAsync(SearchConnections.PurchaseTicketRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/tickets/purchase", request);
            return response;
        }
    }
}

