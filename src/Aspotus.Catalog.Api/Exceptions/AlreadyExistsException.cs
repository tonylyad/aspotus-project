namespace Aspotus.Catalog.Api.Exceptions;

/// <summary>
/// Исключение, выбрасываемое, когда сущность с такими данными уже существует.
/// </summary>
public class AlreadyExistsException : AppException
{
    /// <summary>
    /// Инициализирует новый экземпляр исключения "сущность уже существует".
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    public AlreadyExistsException(string message) : base(message)
    {
    }
}