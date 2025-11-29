using BasketService.API.Helpers;
using BasketService.API.Middleware;
using BasketService.Application.Interfaces;
using BasketService.Application.Mappings;
using BasketService.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    LogHelper.LogStartup("Basket Service API", version, environment);

    var startupStopwatch = Stopwatch.StartNew();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    LogHelper.LogConfiguration("Loading application configuration...");

    // Add services to the container
    builder.Services.AddControllers();
    LogHelper.LogPackage("ASP.NET Core Controllers", "Registered");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "BasketService API", Version = "v1" });
    });
    LogHelper.LogPackage("Swagger", "Registered");

    // Redis Configuration
    LogHelper.LogProcess("Configuring Redis connection...");
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = ConfigurationOptions.Parse(redisConnectionString ?? "localhost:6379", true);
        return ConnectionMultiplexer.Connect(configuration);
    });
    LogHelper.LogRedis($"Connection: {redisConnectionString ?? "localhost:6379"}");

    // MediatR
    LogHelper.LogProcess("Registering MediatR...");
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(
            typeof(BasketService.Application.Features.Baskets.Queries.GetBasket.GetBasketQuery).Assembly);
        cfg.AddOpenBehavior(typeof(BasketService.Application.Behaviors.LoggingBehavior<,>));
    });
    LogHelper.LogPackage("MediatR with LoggingBehavior", "Registered");

    // AutoMapper
    LogHelper.LogProcess("Registering AutoMapper...");
    builder.Services.AddAutoMapper(typeof(MappingProfile));
    LogHelper.LogPackage("AutoMapper", "Registered");

    // FluentValidation
    LogHelper.LogProcess("Registering FluentValidation...");
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(
        typeof(BasketService.Application.Features.Baskets.Queries.GetBasket.GetBasketQuery).Assembly);
    LogHelper.LogPackage("FluentValidation", "Registered");

    // Repository
    LogHelper.LogProcess("Registering Repositories...");
    builder.Services.AddScoped<IBasketRepository, BasketRepository>();
    LogHelper.LogPackage("BasketRepository", "Registered");

    // CORS
    LogHelper.LogProcess("Configuring CORS...");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
    LogHelper.LogPackage("CORS", "Configured");

    var app = builder.Build();

    LogHelper.LogProcess("Configuring HTTP request pipeline...");

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketService API v1"));
        LogHelper.LogApi("Swagger UI enabled");
    }

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    LogHelper.LogPackage("Middleware", "Configured");

    app.UseCors("AllowAll");

    app.UseAuthorization();

    app.MapControllers();

    startupStopwatch.Stop();
    var totalStartupTime = startupStopwatch.ElapsedMilliseconds;

    LogHelper.LogSuccess($"Application started successfully!");
    LogHelper.LogTimer("Total Startup Time", totalStartupTime);
    LogHelper.LogApi($"Listening on: http://localhost:5002");

    Log.Information("🎉 ========================================");
    Log.Information("🎉   BASKET SERVICE IS READY TO USE!    ");
    Log.Information("🎉 ========================================");

    var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStopping.Register(() =>
    {
        Log.Information("🛑 Application is stopping...");
    });

    lifetime.ApplicationStopped.Register(() =>
    {
        LogHelper.LogShutdown();
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    LogHelper.LogError(ex, "Application terminated unexpectedly");
    Log.Fatal(ex, "💥 Application start-up failed!");
}
finally
{
    Log.Information("📝 Flushing logs...");
    await Log.CloseAndFlushAsync();
}
