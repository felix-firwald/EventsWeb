using EventsWeb.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EventsWeb.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                await this.HandleException(httpContext, ex);
            }
        }
        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception. Method={Method}, Path={Path}", httpContext.Request.Method, httpContext.Request.Path);
            if (httpContext.Response.HasStarted)
            {
                return;
            }
            int statusCode = StatusCodeMapping(ex);
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";
            ProblemDetails error = new()
            {
                Status = statusCode,
                Detail = ex.Message
            };
            await httpContext.Response.WriteAsJsonAsync(error);
        }
        private static int StatusCodeMapping(Exception ex)
        {
            return ex switch
            {
                ValidationException ve => StatusCodes.Status400BadRequest,
                NotFoundException nfe => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
