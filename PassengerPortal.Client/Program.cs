/*using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PassengerPortal.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5300/") });

await builder.Build().RunAsync();*/
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PassengerPortal.Client;
using PassengerPortal.Client.Services;
using Microsoft.Extensions.Logging;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Rejestracja HttpClient z adresem backendu
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7075/") });

// Rejestracja ApiService
builder.Services.AddScoped<ApiService>();

// Dodanie logowania
//builder.Logging.SetMinimumLevel(LogLevel.Information);
//builder.Logging.AddConsole();

await builder.Build().RunAsync();

