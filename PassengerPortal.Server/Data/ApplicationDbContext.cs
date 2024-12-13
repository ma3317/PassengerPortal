using Microsoft.EntityFrameworkCore;
using PassengerPortal.Shared.Models;

namespace PassengerPortal.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
    }
}