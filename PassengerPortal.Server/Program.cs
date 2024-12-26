/*using Microsoft.EntityFrameworkCore;
using PassengerPortal.Server.Data;
using PassengerPortal.Shared.Models;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Server.Repositories;
using PassengerPortal.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Dodajemy kontekst bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//---


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7148") // Adres frontendu
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();
//app.UseHttpsRedirection();//////


app.UseCors("AllowFrontend");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();




using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Stations.Any())
    {
        context.Stations.AddRange(
            new Station { Name = "Jaslo", Location = "Jaslo" }
        );
        context.SaveChanges();
    }
}


app.Run();*/

using System.Text.Json.Serialization;
using Route = PassengerPortal.Shared.Models.Route;
using Microsoft.EntityFrameworkCore;
using PassengerPortal.Server.Data;
using PassengerPortal.Shared.Models;
using PassengerPortal.Shared.Interfaces;
using PassengerPortal.Server.Repositories;
using PassengerPortal.Server.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignoruj cykle referencyjne
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Zwiększ maksymalną głębokość serializacji
        options.JsonSerializerOptions.MaxDepth = 32; // Domyślnie 32, ale może być inne w Twojej konfiguracji
        options.JsonSerializerOptions.WriteIndented = true; // Opcjonalne, dla czytelniejszego JSON
    });


// Dodajemy kontekst bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Rejestracja repozytoriów
builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();

// Rejestracja strategii wyszukiwania połączeń
builder.Services.AddScoped<ISearchStrategy, FastestConnectionStrategy>();

// Konfiguracja CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7148")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassengerPortal API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassengerPortal API v1");
    });
}

// Inicjalizacja bazy danych
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Rozpoczynam migrację bazy danych...");
        context.Database.Migrate();

        if (!context.Stations.Any())
        {
            logger.LogInformation("Inicjalizowanie danych dla stacji...");
            context.Stations.AddRange(
                new Station { Name = "Jaslo", City = "Jaslo" },
                new Station { Name = "Krakow", City = "Krakow" },
                new Station { Name = "Warszawa", City = "Warszawa" }
            );
            context.SaveChanges();
            logger.LogInformation("Stacje zostały zainicjalizowane.");
        }

        if (!context.Routes.Any())
{
    logger.LogInformation("Inicjalizowanie danych dla tras...");
    var stations = context.Stations.ToList();

    var jasloStation = stations.FirstOrDefault(s => s.Name == "Jaslo");
    var krakowStation = stations.FirstOrDefault(s => s.Name == "Krakow");
    var warszawaStation = stations.FirstOrDefault(s => s.Name == "Warszawa");

    if (jasloStation == null || krakowStation == null || warszawaStation == null)
    {
        logger.LogWarning("Nie można znaleźć wymaganych stacji: 'Jaslo', 'Krakow', 'Warszawa'.");
    }
    else
    {
        context.Routes.AddRange(
            new Route
            {
                StartStation = jasloStation,
                EndStation = krakowStation,
                Duration = TimeSpan.FromHours(3),
                Timetables = new List<Timetable>
                {
                    new Timetable
                    {
                        DepartureTime = TimeSpan.FromHours(6), // 6:00 rano
                        ArrivalTime = TimeSpan.FromHours(9)   // 9:00 rano
                    },
                    new Timetable
                    {
                        DepartureTime = TimeSpan.FromHours(15), // 15:00
                        ArrivalTime = TimeSpan.FromHours(18)   // 18:00
                    }
                }
            },
            new Route
            {
                StartStation = krakowStation,
                EndStation = warszawaStation,
                Duration = TimeSpan.FromHours(2.5),
                Timetables = new List<Timetable>
                {
                    new Timetable
                    {
                        DepartureTime = TimeSpan.FromHours(10), // 10:00 rano
                        ArrivalTime = TimeSpan.FromHours(12.5) // 12:30
                    },
                    new Timetable
                    {
                        DepartureTime = TimeSpan.FromHours(20), // 20:00
                        ArrivalTime = TimeSpan.FromHours(22.5) // 22:30
                    }
                }
            }
        );
        context.SaveChanges();
        logger.LogInformation("Trasy zostały zainicjalizowane.");
    }
}


    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Wystąpił błąd podczas inicjalizacji bazy danych.");
        throw;
    }
}

app.Run();
