using Aspotus.Gateway.Authorization;
using Aspotus.Gateway.Proxy.Transforms;
using System.Security.Claims;

namespace Aspotus.Gateway.Extensions;

/// <summary>
/// Методы расширения для настройки proxy-маршрутов Gateway.
///</summary>
public static class ProxyExtensions
{
    /// <summary>
    /// Регистрирует proxy-маршруты Gateway, проверяет доступ и пробрасывает данные пользователя в заголовки.
    /// </summary>
    public static IEndpointRouteBuilder MapGatewayProxy(this IEndpointRouteBuilder app)
    {
        app.MapReverseProxy(proxyPipeline =>
        {
            proxyPipeline.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                var method = context.Request.Method;

                var rule = GatewayAccessRules.Find(path, method);

                if (rule is not null)
                {
                    if (!rule.AllowAnonymous)
                    {
                        if (context.User.Identity?.IsAuthenticated != true)
                        {
                            var acceptsHtml = context.Request.Headers.Accept.ToString()
                                .Contains("text/html", StringComparison.OrdinalIgnoreCase);

                            if (acceptsHtml)
                            {
                                context.Response.Redirect("/login.html");
                                return;
                            }

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsJsonAsync(new
                            {
                                message = "Для доступа к ресурсу требуется авторизация."
                            });
                            return;
                        }

                        var userRoles = context.User.FindAll(ClaimTypes.Role)
                            .Select(x => x.Value)
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var hasAllowedRole = rule.AllowedRoles.Length == 0 ||
                                             rule.AllowedRoles.Any(userRoles.Contains);

                        if (!hasAllowedRole)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsJsonAsync(new
                            {
                                message = "Недостаточно прав для выполнения данного действия."
                            });
                            return;
                        }
                    }
                }

                UserHeadersTransform.AddUserHeaders(context);
                await next();
            });
        });

        return app;
    }
}