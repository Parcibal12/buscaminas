using Minesweeper.Api.Services;      // <-- IMPORTANTE: para IGameService y GameService
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Registra GameService
builder.Services.AddSingleton<IGameService, GameService>();

// Configura CORS para permitir llamadas desde tu cliente Angular
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Añade controladores
builder.Services.AddControllers();

var app = builder.Build();

// Habilita CORS
app.UseCors();

// Mapea rutas de API
app.MapControllers();

// Ejecuta la aplicación
app.Run();
