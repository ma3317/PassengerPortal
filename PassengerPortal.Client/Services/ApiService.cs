using PassengerPortal.Shared.Models; // Zamiast PassengerPortal.Client.Models
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
            return await _httpClient.GetFromJsonAsync<List<Station>>("api/stations");
        }
        
        public async Task<List<Connection>> SearchConnectionsAsync(int startStationId, int endStationId, DateTime departureTime)
        {
            var url = $"api/connections/search?startStationId={startStationId}&endStationId={endStationId}&departureTime={departureTime:o}";
            return await _httpClient.GetFromJsonAsync<List<Connection>>(url);
        }
    }
}