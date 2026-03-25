namespace Aspotus.Catalog.Api.Exceptions;

/// <summary>
/// Исключение, выбрасываемое при нарушении бизнес-валидации.
/// </summary>
public class ValidationException : AppException
{
    /// <summary>
    /// Инициализирует новый экземпляр исключения бизнес-валидации.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    public ValidationException(string message) : base(message)
    {
    }
}