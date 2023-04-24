using CoinInvest.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CoinInvest.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong. Sorry.");
            }
        }
    }
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseExceptionHandler(error => error.Run(async context =>
            {
                context.Response.ContentType = "application/json";
            }));
        }
    }
}
