using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.API.Consumers;
using ProductService.API.Helpers;
using ProductService.API.Middleware;
using ProductService.Application.Behaviors;
using ProductService.Application.Interfaces;
using ProductService.Application.Mappings;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using TechnologyStore.Shared;
using TechnologyStore.Shared.Constants;
using TechnologyStore.Shared.Events.Orders;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .CreateLogger();

try
{
    var assembly = Assembly.GetExecutingAssembly();
    var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    LogHelper.LogStartup("Product Service API", version, environment);

    var startupStopwatch = Stopwatch.StartNew();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    LogHelper.LogConfiguration("Loading application configuration...");

    builder.Services.AddControllers();
    LogHelper.LogPackage("ASP.NET Core Controllers", "Registered");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    LogHelper.LogPackage("Swagger", "Registered");

    LogHelper.LogProcess("Registering MediatR...");
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(ProductService.Application.AssemblyReference).Assembly);
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });
    LogHelper.LogPackage("MediatR with Behaviors", "Registered");

    builder.Services.AddValidatorsFromAssembly(typeof(ProductService.Application.AssemblyReference).Assembly);
    LogHelper.LogPackage("FluentValidation", "Registered");

    builder.Services.AddAutoMapper(typeof(MappingProfile));
    LogHelper.LogPackage("AutoMapper", "Registered");

    LogHelper.LogDatabase("Configuring database connection...");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ProductDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
    LogHelper.LogDatabase($"Connection configured: {connectionString?.Split(';')[0]}");

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    LogHelper.LogPackage("Repositories", "Registered");

    LogHelper.LogProcess("Configuring MassTransit with RabbitMQ...");
    builder.Services.AddMassTransit(x =>
    {
        // OrderCreatedConsumer'ı ekle
        x.AddConsumer<OrderCreatedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

            cfg.Host(rabbitMqHost, "/", h =>
            {
                h.Username(rabbitMqUser);
                h.Password(rabbitMqPass);
            });

            // MassTransit otomatik endpoint oluşturacak (IOrderCreatedEvent için)
            cfg.ConfigureEndpoints(context);
        });
    });
    LogHelper.LogPackage("MassTransit + RabbitMQ", $"Configured (Host: {builder.Configuration["RabbitMQ:Host"] ?? "localhost"})");
    LogHelper.LogPackage("OrderCreatedConsumer", "Registered (Consumes: IOrderCreatedEvent)");

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
    LogHelper.LogPackage("CORS Policy", "Registered");

    startupStopwatch.Stop();
    LogHelper.LogTimer("Service Configuration", startupStopwatch.ElapsedMilliseconds);

    var app = builder.Build();

    LogHelper.LogProcess("Configuring middleware pipeline...");

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    LogHelper.LogPackage("Global Exception Handler", "Added to pipeline");

    app.UseMiddleware<RequestLoggingMiddleware>();
    LogHelper.LogPackage("Request Logging", "Added to pipeline");

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API V1");
        c.RoutePrefix = string.Empty;
    });
    LogHelper.LogApi("Swagger UI enabled at: http://localhost:5000");

    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();

    LogHelper.LogDatabase("Applying database migrations...");
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        var migrationStopwatch = Stopwatch.StartNew();

        await db.Database.MigrateAsync();

        migrationStopwatch.Stop();
        LogHelper.LogTimer("Database Migration", migrationStopwatch.ElapsedMilliseconds);
        LogHelper.LogSuccess("Database is ready!");
    }

    var totalStartupTime = startupStopwatch.ElapsedMilliseconds;
    LogHelper.LogSuccess($"Application started successfully!");
    LogHelper.LogTimer("Total Startup Time", totalStartupTime);
    LogHelper.LogApi($"Listening on: {string.Join(", ", builder.Configuration.GetSection("Urls").Get<string[]>() ?? new[] { "http://localhost:5000" })}");

    Log.Information("🎉 ========================================");
    Log.Information("🎉   PRODUCT SERVICE IS READY TO USE!    ");
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
