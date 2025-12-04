using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("🚀 Starting API Gateway...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog();

    // Ocelot configuration
    builder.Configuration
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

    // Add services
    builder.Services.AddOcelot()
        .AddPolly(); 

    // Swagger for Ocelot
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerForOcelot(builder.Configuration);

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Middleware pipeline
    app.UseCors();

    // Swagger UI for all microservices
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
        opt.ReConfigureUpstreamSwaggerJson = AlterUpstreamSwaggerJson;
    });

    // Ocelot middleware
    await app.UseOcelot();

    Log.Information("✅ API Gateway started successfully!");
    Log.Information("🌐 Gateway is listening on: http://localhost:5050");
    Log.Information("📚 Swagger UI: http://localhost:5050/swagger");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "💥 API Gateway start-up failed!");
    throw;
}
finally
{
    Log.Information("🛑 API Gateway shutting down...");
    await Log.CloseAndFlushAsync();
}

static string AlterUpstreamSwaggerJson(HttpContext context, string swaggerJson)
{
    return swaggerJson.Replace("localhost:5000", "localhost:5050")
                      .Replace("localhost:5001", "localhost:5050");
}

