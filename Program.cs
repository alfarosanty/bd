using BlumeAPI.Repository;
using BlumeAPI.Services;
using BlumeAPI.servicios;
using Entities.Repository;
using Entities.servicios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configurar Serilog para escribir logs en archivo por día
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:8082",    // desarrollo
                    "http://192.168.1.104:8081" // producción / otra PC
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
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

        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.SlidingExpiration = true;

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
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IColorService, ColorService>();


builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddScoped<IArticuloRepository, ArticuloRepository>();
builder.Services.AddScoped<IArticuloPrecioRepository, ArticuloPrecioRepository>();
builder.Services.AddScoped<IArticuloService, ArticuloService>();

builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IFacturaService, FacturaServices>();

builder.Services.AddScoped<IIngresoRepository, IngresoRepository>();
builder.Services.AddScoped<IIngresoService, IngresoService>();

builder.Services.AddScoped<IMedidaRepository, MedidaRepository>();
builder.Services.AddScoped<IMedidaService, MedidaServices>();

builder.Services.AddScoped<IPedidoProduccionRepository, PedidoProduccionRepository>();
builder.Services.AddScoped<IPedidoProduccionService, PedidoProduccionService>();

builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoServices>();

builder.Services.AddScoped<ISubFamiliaRepository, SubFamiliaRepository>();
builder.Services.AddScoped<ISubFamiliaService, SubFamiliaServices>();

builder.Services.AddScoped<ITallerRepository, TallerRepository>();
builder.Services.AddScoped<ITallerService, TallerServices>();


// 🔹 Conexión a la base de datos: cambiás manualmente según quieras producción o pruebas
// Para producción:
var connectionString = builder.Configuration.GetConnectionString(
    "BDPruebas"
    //"BDProduccion"
    );

// Para pruebas/desarrollo:
// var connectionString = builder.Configuration.GetConnectionString("BDPruebas");

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
