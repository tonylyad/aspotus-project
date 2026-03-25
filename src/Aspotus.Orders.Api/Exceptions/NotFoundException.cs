namespace Aspotus.Orders.Api.Exceptions;

/// <summary>
/// Исключение, выбрасываемое, когда сущность не найдена.
/// </summary>
public class NotFoundException : AppException
{
    /// <summary>
    /// Инициализирует новый экземпляр исключения "сущность не найдена".
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    public NotFoundException(string message) : base(message)
    {
    }
}