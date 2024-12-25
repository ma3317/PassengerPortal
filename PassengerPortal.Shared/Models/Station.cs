using System.Text.Json.Serialization;

namespace PassengerPortal.Shared.Models
{
    /*public class Station
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }*/
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        [JsonIgnore] 
        public IList<Route> Routes { get; set; } = new List<Route>();
    }
}