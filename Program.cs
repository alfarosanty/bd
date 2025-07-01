using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configurar Serilog para escribir logs en archivo por día
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🌐 Configurar CORS para permitir TODO (⚠️ uso solo si estás seguro)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.WebHost.UseUrls("http://0.0.0.0:7166");

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Swagger (opcional en producción)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Aplicar política CORS global
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
