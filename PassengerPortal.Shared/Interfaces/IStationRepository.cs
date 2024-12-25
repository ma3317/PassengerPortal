using PassengerPortal.Shared.Models;
using System.Collections.Generic;

namespace PassengerPortal.Shared.Interfaces
{
    public interface IStationRepository
    {
        Station GetById(int id);
        IEnumerable<Station> GetAll();
        Station GetByName(string name);
    }
}