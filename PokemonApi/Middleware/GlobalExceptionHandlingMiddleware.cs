using System.Net;
using System.Text.Json;

namespace PokemonApi.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // If using custom exceptions, can use "if" statements to custom handle depending on exception type.

            var status = HttpStatusCode.InternalServerError;
            var resultMessage = "The application encountered an error while processing the request.";

            var exceptionResult = JsonSerializer.Serialize(new { Error = resultMessage });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(exceptionResult);
        }
    }
}