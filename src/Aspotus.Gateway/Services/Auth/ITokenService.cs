using Aspotus.Gateway.Data.Entities;

namespace Aspotus.Gateway.Services.Auth;

/// <summary>
/// Сервис генерации JWT-токенов.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Генерирует JWT-токен для пользователя.
    /// </summary>
    /// <param name="user">Пользователь, для которого создаётся токен.</param>
    /// <returns>Строковое представление JWT-токена.</returns>
    Task<string> GenerateTokenAsync(ApplicationUser user);
}