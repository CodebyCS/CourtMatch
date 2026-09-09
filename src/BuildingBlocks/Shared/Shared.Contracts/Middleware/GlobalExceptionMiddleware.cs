using Microsoft.AspNetCore.Http;
using Shared.Contracts.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Middleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Erro de negócio: {Message}", ex.Message);
                await WriteProblemAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                await WriteProblemAsync(context, 500, "Ocorreu um erro inesperado. Tenta novamente.");
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Title = title,
                Status = statusCode
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
