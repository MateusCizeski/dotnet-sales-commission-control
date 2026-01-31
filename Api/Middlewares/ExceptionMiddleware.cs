using Application.Exceptions;
using Domain.Exceptions;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessRuleException ex)
            {
                await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await HandleException(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DomainException ex)
            {
                await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                await HandleException(context, HttpStatusCode.InternalServerError, "Erro interno inesperado");
            }
        }

        private static async Task HandleException(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            var response = new
            {
                error = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
