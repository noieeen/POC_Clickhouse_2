using System.Reflection;
using AuthService.Models;
using AuthService.Services;
using Core;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        ["module.name"] = "Auth Service"
    });

// From Core
builder.AddServiceDefaults(resourceBuilder);

// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add controllers
builder.Services.AddControllers();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        // DbInitializer.Initialize(context);
    }

    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "An error occurred creating the DB.");
    }
}

// app.UseRouting();
// app.UseHttpsRedirection(); //ERROR: Failed to determine the https port for redirect.
app.UseAuthentication();
app.UseAuthorization(); // Add it here

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapPost("/register-service", async (IServiceProvider scopeFactory, RegisterRequest req) =>
    {
        using var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        var userService = services.GetRequiredService<IUserService>();

        var result = await userService.RegisterUserAsync(req);
        return result;
    })
    .WithName("Register User")
    .WithOpenApi();

app.MapPost("/mock-register-service", async (IServiceProvider scopeFactory) =>
    {
        using var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        var userService = services.GetRequiredService<IUserService>();

        var req = new RegisterRequest
        {
            Username = $"AA{Guid.NewGuid():N}",
            Password = "AAAAA",
            Email = $"{Guid.NewGuid():N}@mail.com"
        };

        var result = await userService.RegisterUserAsync(req);
        return result;
    })
    .WithName("Mock Register User")
    .WithOpenApi();


app.MapDefaultEndpoints(); // Health check Endpoint
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}