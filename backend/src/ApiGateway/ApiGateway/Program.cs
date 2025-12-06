using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Serilog;
using System.Text;

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

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // Swagger with JWT Authorization
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "API Gateway",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

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
    app.UseAuthentication();
    app.UseAuthorization();

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

