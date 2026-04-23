namespace Aspotus.Orders.Api.Extensions;

/// <summary>
/// Методы расширения для чтения пользовательских данных из заголовков запроса.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Возвращает идентификатор пользователя из заголовка запроса.
    /// </summary>
    /// <param name="httpContext">Текущий HTTP-контекст.</param>
    /// <returns>Идентификатор пользователя или null, если заголовок отсутствует.</returns>
    public static string? GetGatewayUserId(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["X-User-Id"].FirstOrDefault();
    }

    /// <summary>
    /// Возвращает электронную почту пользователя из заголовка запроса.
    /// </summary>
    /// <param name="httpContext">Текущий HTTP-контекст.</param>
    /// <returns>Электронная почта пользователя или null, если заголовок отсутствует.</returns>
    public static string? GetGatewayUserEmail(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["X-User-Email"].FirstOrDefault();
    }

    /// <summary>
    /// Возвращает полное имя пользователя из заголовка запроса.
    /// </summary>
    /// <param name="httpContext">Текущий HTTP-контекст.</param>
    /// <returns>Полное имя пользователя или null, если заголовок отсутствует.</returns>
    public static string? GetGatewayUserFullName(this HttpContext httpContext)
    {
        return httpContext.Request.Headers["X-User-FullName"].FirstOrDefault();
    }

    /// <summary>
    /// Возвращает список ролей пользователя из заголовка запроса.
    /// </summary>
    /// <param name="httpContext">Текущий HTTP-контекст.</param>
    /// <returns>Список ролей пользователя.</returns>
    public static IReadOnlyCollection<string> GetGatewayRoles(this HttpContext httpContext)
    {
        var rolesHeader = httpContext.Request.Headers["X-User-Roles"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(rolesHeader))
        {
            return Array.Empty<string>();
        }

        return rolesHeader
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }

    /// <summary>
    /// Проверяет, есть ли у пользователя указанная роль.
    /// </summary>
    /// <param name="httpContext">Текущий HTTP-контекст.</param>
    /// <param name="role">Название роли.</param>
    /// <returns>True, если роль найдена, иначе false.</returns>
    public static bool HasGatewayRole(this HttpContext httpContext, string role)
    {
        return httpContext.GetGatewayRoles()
            .Any(x => string.Equals(x, role, StringComparison.OrdinalIgnoreCase));
    }
}