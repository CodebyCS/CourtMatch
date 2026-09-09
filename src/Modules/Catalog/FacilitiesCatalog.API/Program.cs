using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Catalog.Application.Services;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Validators;
using FluentValidation;
using System.Text.Json.Serialization;
using Shared.Contracts.Middleware;
using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Clients;

namespace FacilitiesCatalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers com serialização de enums como texto ("Active" e "UnderMaitenance")

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Cola apenas o token JWT."
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference =
                                    new Microsoft.OpenApi.Models.OpenApiReference
                                    {
                                        Type =
                                            Microsoft.OpenApi.Models.ReferenceType
                                                .SecurityScheme,
                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });
            // Base de dados PostgreSQL (supabase)
            builder.Services.AddDbContext<CatalogDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Registo de Repositórios e Serviços
            builder.Services.AddScoped<ICourtRepository, CourtRepository>();
            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

            builder.Services.AddScoped<ICourtService, CourtService>();
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();

            // Registo de validadores (FluentValidation)
            builder.Services.AddValidatorsFromAssemblyContaining<CreateCourtRequestValidator>();

            var jwtSecret = builder.Configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException(
                    "JwtSettings:Secret não foi configurada.");

            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
                ?? "CourtMatch";

            var jwtAudience = builder.Configuration["JwtSettings:Audience"]
                ?? "CourtMatchClient";

            builder.Services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecret)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            builder.Services.AddAuthorization();

            // Middleware global de tratamento de exceções
            builder.Services.AddGlobalExceptionHandling();

            var gameApiBaseUrl =
                builder.Configuration["Services:GameApi:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "O endereço da Game API não foi configurado.");

            builder.Services.AddHttpClient<
                IGameAvailabilityClient,
                GameAvailabilityClient>(client =>
            {
                client.BaseAddress = new Uri(gameApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Ativação do middleware global de erros
            app.UseGlobalExceptionHandling();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
