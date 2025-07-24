using ApiSAPBridge.Core.Interfaces;
using ApiSAPBridge.Services.Implementations;
using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Data;
using ApiSAPBridge.Data.Extensions;

//using ApiSAPBridge.Data.UnitOfWork;
using ApiSAPBridge.Models.Configuration;
using ApiSAPBridge.Models.Constants;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using ApiSAPBridge.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


// ===== CONFIGURACIÓN DE BASE DE DATOS =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Log de la connection string (sin password) para debug
var maskedConnectionString = System.Text.RegularExpressions.Regex.Replace(
    connectionString,
    @"(Password|Pwd)=([^;]*)",
    "$1=***",
    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

Console.WriteLine($"Using connection string: {maskedConnectionString}");

// Configurar DbContext
builder.Services.AddDbContext<ApiSAPBridgeDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("ApiSAPBridge.Data");
        sqlOptions.CommandTimeout(120);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    });

    // Configuración adicional para development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(true);
        options.EnableDetailedErrors(true);
    }

    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "ApiLogs",
            AutoCreateSqlTable = true
        })
    .CreateLogger();

builder.Host.UseSerilog();

// Configurar servicios
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Personalizar respuestas de validación
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new
            {
                Success = false,
                Message = ApiConstants.ResponseMessages.ERROR_VALIDATION,
                Errors = errors,
                Timestamp = DateTime.UtcNow
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
        };
    });

// Configurar Entity Framework
builder.Services.AddApiSAPBridgeData(builder.Configuration);

// Configurar Unit of Work - LÍNEA CRÍTICA
builder.Services.AddScoped<ApiSAPBridge.Core.Interfaces.IUnitOfWork, ApiSAPBridge.Data.UnitOfWork.UnitOfWork>();

// Registrar servicios de negocio
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<ISeccionService, SeccionService>();
builder.Services.AddScoped<IFamiliaService, FamiliaService>();
builder.Services.AddScoped<IVendedorService, VendedorService>();
builder.Services.AddScoped<IImpuestoService, ImpuestoService>(); 
builder.Services.AddScoped<IFormaPagoService, FormaPagoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITarifaService, TarifaService>();
builder.Services.AddScoped<IArticuloService, ArticuloService>();          
builder.Services.AddScoped<IArticuloLineaService, ArticuloLineaService>(); 
builder.Services.AddScoped<IPrecioService, PrecioService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();


// Configurar Mapster
//builder.Services.AddMapster();
//TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);
// Configurar Mapster
ApiSAPBridge.API.Mapping.MapsterConfig.Configure();

// Configurar opciones
builder.Services.Configure<ApiKeyConfig>(
    builder.Configuration.GetSection("ApiKeys"));

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API SAP Bridge",
        Version = "v1",
        Description = "API para integración con sistema SAP - Recepción y consulta de datos"
    });

    // Configurar autenticación en Swagger
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-Api-Key: {key}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = ApiConstants.API_KEY_HEADER,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityDefinition("AuthToken", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Auth Token needed for tariffs endpoints. X-Auth-Token: {token}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = ApiConstants.AUTH_TOKEN_HEADER,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar middleware de autenticación personalizada
//builder.Services.AddScoped<ApiKeyAuthenticationMiddleware>();

//  logging detallado
builder.Services.AddDbContext<ApiSAPBridgeDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API SAP Bridge v1");
        c.DefaultModelsExpandDepth(-1); // Ocultar modelos por defecto
    });
}

// Usar Serilog para logging de requests
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

// Middleware personalizado de autenticación
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Healthcheck endpoint
app.MapGet("/health", async (ApiSAPBridgeDbContext context) =>
{
    try
    {
        var canConnect = await context.TestConnectionAsync();
        var dbInfo = await context.GetDatabaseInfoAsync();

        return Results.Ok(new
        {
            Status = canConnect ? "Healthy" : "Unhealthy",
            Database = new
            {
                CanConnect = dbInfo.CanConnect,
                DatabaseExists = dbInfo.DatabaseExists,
                PendingMigrations = dbInfo.PendingMigrations.Count(),
                AppliedMigrations = dbInfo.AppliedMigrations.Count()
            },
            Timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Health Check Failed");
    }
});

// Asegurar que la base de datos esté actualizada
try
{
    await app.Services.EnsureDatabaseAsync();
    Log.Information("Aplicación iniciada correctamente");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error al inicializar la aplicación");
    throw;
}

app.Run();