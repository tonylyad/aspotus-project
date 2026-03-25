namespace Aspotus.Catalog.Api.Exceptions;

/// <summary>
/// Базовое пользовательское исключение приложения.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр пользовательского исключения приложения.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    protected AppException(string message) : base(message)
    {
    }
}