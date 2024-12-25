namespace PassengerPortal.Shared.Models;

public class Passenger : User
{
    public bool IsStudent { get; set; }

    public Passenger()
    {
        Roles.Add(new Role { RoleName = "Passenger" });
    }
}