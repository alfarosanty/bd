using System.Data;
using System.Globalization;
using BlumeAPI.Repositories;
using BlumeAPI.Services;
using BlumeAPI.Services.Imp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configurar Serilog para escribir logs en archivo por día
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

CultureInfo custom = (CultureInfo)CultureInfo.InvariantCulture.Clone();
custom.NumberFormat.NumberDecimalSeparator = ".";

CultureInfo.DefaultThreadCurrentCulture = custom;
CultureInfo.DefaultThreadCurrentUICulture = custom;

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:8080", // desarrollo Compu Agos
                    "http://localhost:8082",    // desarrollo notebook Santi
                    "http://192.168.1.104:8081" // producción / otra PC
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");

        });
});

builder.WebHost.UseUrls("http://0.0.0.0:7166");

// Servicios
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication("MiCookieAuth")
    .AddCookie("MiCookieAuth", options =>
    {
        options.Cookie.Name = "MiSistema.Cookie";
        options.LoginPath = "/api/auth/login";
        options.AccessDeniedPath = "/api/auth/denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;

        options.ExpireTimeSpan = TimeSpan.FromHours(9);
        options.SlidingExpiration = false;


        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IArticuloRepository, ArticuloRepository>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IMedidaRepository, MedidaRepository>();
builder.Services.AddScoped<ISubfamiliaRepository, SubfamiliaRepository>();

// Servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<IArticuloService, ArticuloServicesNUEVO>();




// 🔹 Conexión a la base de datos: cambiás manualmente según quieras producción o pruebas
var connectionString = builder.Configuration.GetConnectionString(
    "BDPruebasPCEri"
    //"BDPruebas"
    //"BDProduccion"
    );

// Configuración de NpgsqlConnection para inyección de dependencias
builder.Services.AddScoped<NpgsqlConnection>(_ =>
    new NpgsqlConnection(connectionString)
);

builder.Services.AddScoped<IDbConnection>(_ =>
    new NpgsqlConnection(connectionString)
);
// Configuración de Entity Framework Core con PostgreSQL

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseRouting(); // 🚀 siempre antes de CORS

// CORS con la política correcta
app.UseCors("CorsPolicy");

// Swagger (opcional en producción)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
