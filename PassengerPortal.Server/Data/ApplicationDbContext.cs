using Microsoft.EntityFrameworkCore;
using PassengerPortal.Shared.Models;
using Route = PassengerPortal.Shared.Models.Route;

namespace PassengerPortal.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Ticket> Tickets { get; set; }//new

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacja Route -> StartStation
            modelBuilder.Entity<Route>()
                .HasOne(r => r.StartStation)
                .WithMany(s => s.Routes)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacja Route -> EndStation
            modelBuilder.Entity<Route>()
                .HasOne(r => r.EndStation)
                .WithMany() // Brak kolekcji w EndStation
                .OnDelete(DeleteBehavior.Restrict);

            // Relacja Timetable -> Route
            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Timetables)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            //new
            // Relacja wiele-do-wielu między Ticket a Route
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Routes)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "TicketRoute",
                    tr => tr.HasOne<Route>().WithMany().HasForeignKey("RouteId"),
                    tr => tr.HasOne<Ticket>().WithMany().HasForeignKey("TicketId"),
                    tr =>
                    {
                        tr.HasKey("TicketId", "RouteId");
                        tr.ToTable("TicketRoutes");
                    });
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.StartStation)
                .WithMany()
                .HasForeignKey(t => t.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.EndStation)
                .WithMany()
                .HasForeignKey(t => t.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);

            //new
        }
    }
}
