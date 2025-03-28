using System.Reflection;
using AuthService.Services;
using Core;
using Core.Factory;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;

// using System.Globalization;
// using System.Runtime.InteropServices;
//
// if (GlobalizationMode.Invariant)
// {
//     throw new Exception("Invariant mode is still enabled! ICU is not recognized.");
// }
//
// CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
// CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

// From Core
builder.AddServiceDefaults(resourceBuilder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
// Database connection
var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserService, UserService>(); // Register the correct implementation

builder.Services
    .AddSingleton<ICommon_Exception_Factory, Common_Exception_Factory>(); // Register the correct implementation
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Register the correct implementation

builder.Services.AddControllers();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");


// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
//
// app.MapGet("/weatherforecast", () =>
//     {
//         var forecast = Enumerable.Range(1, 5).Select(index =>
//                 new WeatherForecast
//                 (
//                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                     Random.Shared.Next(-20, 55),
//                     summaries[Random.Shared.Next(summaries.Length)]
//                 ))
//             .ToArray();
//         return forecast;
//     })
//     .WithName("GetWeatherForecast")
//     .WithOpenApi();
app.MapDefaultEndpoints();
app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }