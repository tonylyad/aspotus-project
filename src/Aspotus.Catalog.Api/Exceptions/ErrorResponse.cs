namespace Aspotus.Catalog.Api.Exceptions;

/// <summary>
/// Стандартная модель ответа с информацией об ошибке.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP-статус ответа.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; set; } = null!;
}
