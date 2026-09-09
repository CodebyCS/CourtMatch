using Game.API;
using Game.Application.Interfaces;
using Game.Application.Services;
using Game.Domain.Repositories;
using Game.Infrastructure.Data;
using Game.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Game.Application.Validators;
using Shared.Contracts.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Base de dados (Supabase = Postgres, via Npgsql) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' não foi configurada.");

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Game")));

// --- Injeção de dependência ---
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerRankingRepository, PlayerRankingRepository>();
builder.Services.AddScoped<IGameService, GameService>();

// Validações
builder.Services.AddValidatorsFromAssemblyContaining<CreateGameDtoValidator>();

// --- Tratamento global de exceções ---
builder.Services.AddProblemDetails();

// --- Controllers & Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Games.API - CourtMatch",
        Version = "v1",
        Description = "Gestão de jogos, participantes, resultados e ranking de jogadores."
    });
});

// --- CORS (ajustar origem conforme o frontend) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddGlobalExceptionHandling();

var app = builder.Build();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCors");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
