using System.Text.Json;
using BussinesMS.Dominio.Excepciones;

namespace BussinesMS.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error no manejado: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        int statusCode = 500;
        string errorCode = "ERROR_INTERNO";
        string mensaje = "Error interno del servidor";

        switch (exception)
        {
            case ExcepcionDominio exDominio:
                statusCode = exDominio.CodigoHttp;
                errorCode = exDominio.CodigoError;
                mensaje = exDominio.Message;
                break;

            case ArgumentNullException:
            case ArgumentException:
                statusCode = 400;
                errorCode = "ARGUMENTO_INVALIDO";
                mensaje = exception.Message;
                break;

            case KeyNotFoundException:
                statusCode = 404;
                errorCode = "NO_ENCONTRADO";
                mensaje = exception.Message;
                break;

            case InvalidOperationException:
                statusCode = 400;
                errorCode = "OPERACION_INVALIDA";
                mensaje = exception.Message;
                break;

            case FluentValidation.ValidationException fluentlyEx:
                statusCode = 400;
                errorCode = "VALIDACION_FLUENT";
                mensaje = string.Join("; ", fluentlyEx.Errors.Select(e => e.ErrorMessage));
                break;
        }

        response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            success = false,
            message = mensaje,
            errorCode = errorCode
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}