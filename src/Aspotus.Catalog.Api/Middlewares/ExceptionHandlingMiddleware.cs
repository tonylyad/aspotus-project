using System.Text.Json;
using Aspotus.Catalog.Api.Exceptions;

namespace Aspotus.Catalog.Api.Middlewares;

/// <summary>
/// Глобальный middleware для централизованной обработки исключений приложения.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Инициализирует новый экземпляр middleware обработки исключений.
    /// </summary>
    /// <param name="next">Следующий делегат конвейера обработки запроса.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Выполняет обработку HTTP-запроса и перехватывает необработанные исключения.
    /// </summary>
    /// <param name="context">Контекст текущего HTTP-запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (AlreadyExistsException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Во время обработки запроса произошла внутренняя ошибка сервера.");
        }
    }

    /// <summary>
    /// Формирует и записывает в ответ стандартную модель ошибки.
    /// </summary>
    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}