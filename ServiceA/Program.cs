using System.Diagnostics;
using System.Reflection;
using AuthService.Services;
using Core;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
// Define service name and version for telemetry
var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName, serviceVersion: serviceVersion);

// From Core
builder.AddServiceDefaults(resourceBuilder);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

// Sample endpoint that generates telemetry
app.MapGet("/sample", async (ILogger<Program> logger) =>
{
    var activitySource = new ActivitySource(serviceName);
    using var activity = activitySource.StartActivity("SampleOperation");

    activity?.SetTag("sample.importance", "high");

    // Log something
    logger.LogInformation("A Sample endpoint called at {Time}", DateTime.UtcNow);

    // Simulate some work
    await Task.Delay(new Random().Next(10, 100));

    return "Hello from monitored .NET app! from A";
});

app.MapGet("/login", async (ILogger<Program> logger) =>
{
    var activitySource = new ActivitySource(serviceName);
    using var activity = activitySource.StartActivity("AuthOperation");

    activity?.SetTag("auth.importance", "high");

    var auth = new Auth();
    var success = await auth.Login("username", "password");

    // Log something
    logger.LogInformation("Login Resp :{Result}", success ? "Success" : "Fail");

    // Simulate some work
    await Task.Delay(new Random().Next(10, 100));

    return "Success";
});
app.Run();