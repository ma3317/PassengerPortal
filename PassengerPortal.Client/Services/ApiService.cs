/*using System.Net.Http.Json;
using PassengerPortal.Shared.Models; // Zakładam, że tam masz Connection, Station, itp.

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

        public async Task<List<Connection>> SearchConnectionsAsync(int startStationId, int endStationId, DateTime departureTime)
        {
            // Format daty w ISO 8601, np. 2024-12-28T09:00:00
            var departureTimeFormatted = departureTime.ToString("o");

            var query = $"api/connections/search?startStationId={startStationId}&endStationId={endStationId}&departureTime={departureTimeFormatted}";

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Connection>>();
        }
    }
}*/
using System.Net.Http.Json;
using PassengerPortal.Shared.Models; // Zakładam, że tam masz Connection, Station, itp.

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

        public async Task<List<Connection>> SearchConnectionsAsync(int startStationId, int endStationId, DateTime departureTime)
        {
            // Format daty w ISO 8601, np. 2024-12-28T09:00:00Z
            var departureTimeFormatted = departureTime.ToUniversalTime().ToString("o");

            var query = $"api/connections/search?startStationId={startStationId}&endStationId={endStationId}&departureTime={departureTimeFormatted}";

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Connection>>();
        }
    }
}
