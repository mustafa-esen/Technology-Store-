using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Consumers;
using PaymentService.API.Helpers;
using PaymentService.API.Middleware;
using PaymentService.Application.Behaviors;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========== SERILOG CONFIGURATION ==========
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/paymentservice-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("═══════════════════════════════════════════════════════════");
logger.LogInformation("🚀 PAYMENT SERVICE STARTING...");
logger.LogInformation("📍 Port: 5004");
logger.LogInformation("🕐 Time: {Time}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"));
logger.LogInformation("═══════════════════════════════════════════════════════════");

// ========== DATABASE CONFIGURATION ==========
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== REPOSITORY & SERVICES ==========
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentGateway, FakePaymentGateway>();

// ========== MEDIATR & BEHAVIORS ==========
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PaymentService.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommand).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// ========== AUTOMAPPER ==========
builder.Services.AddAutoMapper(typeof(PaymentService.Application.Mappings.MappingProfile));

// ========== FLUENT VALIDATION ==========
builder.Services.AddValidatorsFromAssembly(
    typeof(PaymentService.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommand).Assembly);

// ========== MASSTRANSIT & RABBITMQ ==========
builder.Services.AddMassTransit(x =>
{
    // Register consumer
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqPort = ushort.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672");
        var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "admin";
        var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "admin123";

        cfg.Host(rabbitMqHost, rabbitMqPort, "/", h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        // Configure endpoint for OrderCreatedConsumer
        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });

        Log.Logger.Information("🐰 RabbitMQ configured: {Host}:{Port}", rabbitMqHost, rabbitMqPort);
        Log.Logger.Information("🐰 Consumer registered: OrderCreatedConsumer -> order-created-queue");
    });
});

// ========== CONTROLLERS & SWAGGER ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Payment Service API", Version = "v1" });
});

// ========== CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// ========== DATABASE MIGRATION ==========
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    try
    {
        Log.Logger.Information("🔄 Applying database migrations...");
        dbContext.Database.Migrate();
        Log.Logger.Information("✅ Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Logger.Error(ex, "❌ Database migration failed");
    }
}

Log.Logger.Information("═══════════════════════════════════════════════════════════");
Log.Logger.Information("✅ PAYMENT SERVICE IS READY!");
Log.Logger.Information("💳 Payment processing: Active");
Log.Logger.Information("🐰 RabbitMQ consumer: Listening");
Log.Logger.Information("📊 Swagger UI: /swagger");
Log.Logger.Information("═══════════════════════════════════════════════════════════");

app.Run();

// Graceful shutdown
Log.Logger.Information("🛑 Payment Service shutting down...");
Log.CloseAndFlush();
