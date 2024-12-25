namespace PassengerPortal.Shared.Models;

public class Admin : User
{
    public Admin()
    {
        Roles.Add(new Role { RoleName = "Admin" });
    }
}