using System.Diagnostics;
using System.Reflection;
using Core;
using OpenTelemetry.Resources;


var builder = WebApplication.CreateBuilder(args);

var serviceName = "ServiceB";

// var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapDefaultEndpoints();

app.MapGet("/sample", async (ILogger<Program> logger) =>
{
    var activitySource = new ActivitySource(serviceName);
    using var activity = activitySource.StartActivity("SampleOperation");

    activity?.SetTag("sample.importance", "high");

    // Log something
    logger.LogInformation("B Sample endpoint called at {Time}", DateTime.UtcNow);

    // Simulate some work
    await Task.Delay(new Random().Next(10, 100));

    return "Hello from monitored .NET app from B!";
});

app.Run();