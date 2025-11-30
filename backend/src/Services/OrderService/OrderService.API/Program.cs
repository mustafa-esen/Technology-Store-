using System.Diagnostics;
using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Consumers;
using OrderService.API.Helpers;
using OrderService.API.Middleware;
using OrderService.Application.Behaviors;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using Serilog;
using TechnologyStore.Shared.Constants;
using TechnologyStore.Shared.Events.Baskets;

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

    LogHelper.LogStartup("Order Service API", version, environment);

    var startupStopwatch = Stopwatch.StartNew();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    LogHelper.LogConfiguration("Loading application configuration...");

    builder.Services.AddControllers();
    LogHelper.LogPackage("ASP.NET Core Controllers", "Registered");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    LogHelper.LogPackage("Swagger", "Registered");

    LogHelper.LogDatabase("Configuring database connection...");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<OrderDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
    LogHelper.LogDatabase($"Connection configured: {connectionString?.Split(';')[0]}");

    LogHelper.LogProcess("Registering MediatR...");
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.Load("OrderService.Application"));
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });
    LogHelper.LogPackage("MediatR with Behaviors", "Registered");

    builder.Services.AddAutoMapper(Assembly.Load("OrderService.Application"));
    LogHelper.LogPackage("AutoMapper", "Registered");

    builder.Services.AddValidatorsFromAssembly(Assembly.Load("OrderService.Application"));
    LogHelper.LogPackage("FluentValidation", "Registered");

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    LogHelper.LogPackage("Repositories", "Registered");

    LogHelper.LogMessageBus("Configuring RabbitMQ...");
    builder.Services.AddMassTransit(x =>
    {
        // Consumer'ı kaydet
        x.AddConsumer<BasketCheckoutConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            var rabbitMqPort = builder.Configuration.GetValue<int>("RabbitMQ:Port");
            if (rabbitMqPort == 0) rabbitMqPort = 5672;
            var rabbitMqUsername = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            var rabbitMqPassword = builder.Configuration["RabbitMQ:Password"] ?? "guest";

            cfg.Host(rabbitMqHost, (ushort)rabbitMqPort, RabbitMqConstants.DefaultVirtualHost, h =>
            {
                h.Username(rabbitMqUsername);
                h.Password(rabbitMqPassword);
            });

            // BasketCheckoutConsumer için endpoint yapılandırması
            cfg.ReceiveEndpoint(RabbitMqConstants.BasketCheckoutQueue, e =>
            {
                // Retry policy: Hata durumunda 3 kez dene
                e.UseMessageRetry(r => r.Interval(RabbitMqConstants.MaxRetryCount, TimeSpan.FromSeconds(RabbitMqConstants.RetryDelaySeconds)));

                // Consumer'ı bu endpoint'e bağla
                e.ConfigureConsumer<BasketCheckoutConsumer>(context);
            });

            // Diğer endpoint'ler için otomatik yapılandırma
            cfg.ConfigureEndpoints(context);
        });
    });
    LogHelper.LogMessageBus($"RabbitMQ configured: {builder.Configuration["RabbitMQ:Host"]}");
    LogHelper.LogMessageBus($"Consumer registered: BasketCheckoutConsumer -> {RabbitMqConstants.BasketCheckoutQueue}");

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API V1");
        c.RoutePrefix = "swagger";
    });
    LogHelper.LogApi("Swagger UI enabled at: http://localhost:5003/swagger");

    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();

    LogHelper.LogDatabase("Applying database migrations...");
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var migrationStopwatch = Stopwatch.StartNew();

        await db.Database.MigrateAsync();

        migrationStopwatch.Stop();
        LogHelper.LogTimer("Database Migration", migrationStopwatch.ElapsedMilliseconds);
        LogHelper.LogSuccess("Database is ready!");
    }

    var totalStartupTime = startupStopwatch.ElapsedMilliseconds;
    LogHelper.LogSuccess($"Application started successfully!");
    LogHelper.LogTimer("Total Startup Time", totalStartupTime);
    LogHelper.LogApi($"Listening on: {string.Join(", ", builder.Configuration.GetSection("Urls").Get<string[]>() ?? new[] { "http://localhost:5003" })}");

    Log.Information("🎉 ========================================");
    Log.Information("🎉   ORDER SERVICE IS READY TO USE!      ");
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


