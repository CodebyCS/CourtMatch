using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Middleware
{
    public static class MiddlewareExtensions
    {
        // Chamar no builder.Services de cada API
        /// <summary>Registers the middleware that converts application exceptions into HTTP responses.</summary>
        /// <param name="services">The API service collection.</param>
        /// <returns>The service collection with the middleware registered.</returns>
        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();
            return services;
        }

        // Chamar logo no início do pipeline (antes de UseAuthentication) em cada API
        /// <summary>Adds global exception handling to the API request pipeline.</summary>
        /// <param name="app">The application to configure.</param>
        /// <returns>The application with the middleware added.</returns>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
