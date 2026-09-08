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
        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();
            return services;
        }

        // Chamar logo no início do pipeline (antes de UseAuthentication) em cada API
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
