using System.Security.Claims;

namespace Aspotus.Gateway.Proxy.Transforms;

/// <summary>
/// Методы для проброса данных пользователя в заголовки запроса.
/// </summary>
public static class UserHeadersTransform
{
    /// <summary>
    /// Добавляет в запрос заголовки с данными текущего пользователя.
    /// </summary>
    /// <param name="context">Текущий HTTP-контекст.</param>
    public static void AddUserHeaders(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = context.User.FindFirstValue(ClaimTypes.Email);
        var roles = context.User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            context.Request.Headers["X-User-Id"] = userId;
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            context.Request.Headers["X-User-Email"] = email;
        }

        if (roles.Length > 0)
        {
            context.Request.Headers["X-User-Roles"] = string.Join(",", roles);
        }
    }
}
