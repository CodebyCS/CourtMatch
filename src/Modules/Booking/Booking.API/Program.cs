using Booking.Application.Interfaces;
using Booking.Application.Services;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Booking.Infrastructure.Clients;
using Shared.Contracts.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Booking.API

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "A connection string 'DefaultConnection' não foi configurada.");

            builder.Services.AddDbContext<BookingDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsHistoryTable(
                        "__EFMigrationsHistory_Booking")));

            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            var catalogBaseUrl =
                builder.Configuration["Services:CatalogApi:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "O endereço da Catalog API não foi configurado.");

            builder.Services.AddHttpClient<
                ICatalogAvailabilityClient,
                CatalogAvailabilityClient>(client =>
            {
                client.BaseAddress = new Uri(catalogBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });

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

            builder.Services.AddGlobalExceptionHandling();

            builder.Services.AddControllers();
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
            });;
            builder.Services.AddProblemDetails();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseGlobalExceptionHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
