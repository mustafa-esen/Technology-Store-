using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReviewService.API.Helpers;
using ReviewService.API.Middleware;
using ReviewService.Application.Interfaces;
using ReviewService.Infrastructure.Data;
using ReviewService.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// MongoDB Configuration
var mongoConnectionString = builder.Configuration["MongoDb:ConnectionString"]
    ?? throw new InvalidOperationException("MongoDB ConnectionString is not configured");
var mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"]
    ?? throw new InvalidOperationException("MongoDB DatabaseName is not configured");

builder.Services.AddSingleton(new MongoDbContext(mongoConnectionString, mongoDatabaseName));
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ReviewService.Application.DTOs.ReviewDto).Assembly);
});

// Validation Pipeline Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ReviewService.Application.Behaviors.ValidationBehavior<,>));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(ReviewService.Application.DTOs.ReviewDto).Assembly);

// AutoMapper
builder.Services.AddAutoMapper(typeof(ReviewService.Application.Mappings.MappingProfile).Assembly);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Review Service API",
        Version = "v1",
        Description = "Review Service API for TechnologyStore"
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

var app = builder.Build();

// Startup Logging
LogHelper.LogStartup("Review Service", "1.0.0", app.Environment.EnvironmentName);
LogHelper.LogConfiguration($"MongoDB: {mongoDatabaseName}");
LogHelper.LogConfiguration($"JWT Issuer: {jwtSettings["Issuer"]}");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    LogHelper.LogApi("Swagger UI enabled at /swagger");
}

// Middleware Pipeline
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

LogHelper.LogSuccess("Review Service is ready to handle requests");

app.Lifetime.ApplicationStopping.Register(() => LogHelper.LogShutdown());

app.Run();
