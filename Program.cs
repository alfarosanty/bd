using System.Data;
using System.Globalization;
using BlumeAPI.Entities.Repository;
using BlumeAPI.Repository;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Serilog;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
                    "http://localhost:8081", // desarrollo Compu Agos
                    "http://localhost:8082",    // desarrollo notebook Santi
                    "http://192.168.1.41:8081", // producción / otra PC
                    "http://192.168.0.101:8081", // producción / otra PC
                    "http://192.168.0.100:8081", // producción / otra PC
                    "http://192.168.0.102:8081",// producción / otra PC
                    "http://192.168.0.103:8081", // producción / otra PC
                    "http://192.168.0.104:8081", // producción / otra PC
                    "http://192.168.0.105:8081" // producción / otra PC
                    



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
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IArticuloRepository, ArticuloRepository>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IMedidaRepository, MedidaRepository>();
builder.Services.AddScoped<ISubfamiliaRepository, SubfamiliaRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddScoped<IPedidoProduccionRepository,PedidoProduccionRepository>();
builder.Services.AddScoped<ITallerRepository, TallerRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IIngresoRepository, IngresoRepository>();
builder.Services.AddScoped<IARCARepository, ARCARepository>();



// Servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<IArticuloService, ArticuloServicesNUEVO>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<IPedidoProduccionService, PedidoProduccionService>();
builder.Services.AddScoped<ITallerService, TallerService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IIngresoService, IngresoService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IMedidaService, MedidaServices>();
builder.Services.AddScoped<ISubfamiliaService, SubfamiliaService>();
builder.Services.AddScoped<IARCAService, ARCAService>();


// 🔹 Conexión a la base de datos: cambiás manualmente según quieras producción o pruebas
var connectionString = builder.Configuration.GetConnectionString(
    "BD"
    );

builder.Services.AddScoped<AfipPadronClient>(provider => 
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<AfipPadronClient>>();
    return new AfipPadronClient(config["Afip:UrlPadronA13"], logger);
});
// Variables de entorno para AFIP WSFE1
builder.Services.Configure<AfipSettings>(builder.Configuration.GetSection("Afip"));

builder.Services.AddScoped<AfipWsfeClient>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<AfipSettings>>().Value;
    return new AfipWsfeClient(settings.UrlWsfev1);
});


/* Configuración de NpgsqlConnection para inyección de dependencias
builder.Services.AddScoped<NpgsqlConnection>(_ =>
    new NpgsqlConnection(connectionString)
);

builder.Services.AddScoped<IDbConnection>(_ =>
    new NpgsqlConnection(connectionString)
);
// Configuración de Entity Framework Core con PostgreSQL
*/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.LogTo(Console.WriteLine, LogLevel.Information);
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

var app = builder.Build();

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { mensaje = "Error interno del servidor" });
    });
});
app.UseMiddleware<ExceptionMiddleware>();


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
