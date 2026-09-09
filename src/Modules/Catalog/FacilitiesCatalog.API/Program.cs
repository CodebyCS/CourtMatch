using Catalog.Application.Services;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Validators;
using FluentValidation;
using System.Text.Json.Serialization;
using Shared.Contracts.Middleware;

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
            builder.Services.AddSwaggerGen();
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

            // Middleware global de tratamento de exceções
            builder.Services.AddGlobalExceptionHandling();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
