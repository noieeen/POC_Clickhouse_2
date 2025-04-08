using System.Reflection;
using Core;
using Core.Factory;
using Core.Services.CacheService;
using Core.Services.ProductService;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";

// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        ["module.name"] = "Store Api"
    });

builder.AddServiceDefaults(resourceBuilder);
builder.AddDefaultLogging();

var connectionString = builder.Configuration.GetConnectionString("DbConnectionString");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductService, ProductService>(); // Register the correct implementation

// Redis
builder.Services.AddDistributedMemoryCache();
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
var redis = ConnectionMultiplexer.Connect(redisConnectionString);

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tr => tr.AddRedisInstrumentation(redis));

// Register the correct implementation
builder.Services
    .AddSingleton<ICommon_Exception_Factory, Common_Exception_Factory>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.MapDefaultEndpoints();

app.Run();