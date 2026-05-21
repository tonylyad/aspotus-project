namespace Aspotus.Gateway.Authorization;

/// <summary>
/// Набор правил доступа к проксируемым маршрутам Gateway.
/// </summary>
public static class GatewayAccessRules
{
    /// <summary>
    /// Возвращает список правил доступа.
    /// </summary>
    public static IReadOnlyCollection<GatewayAccessRule> GetRules() =>
        new List<GatewayAccessRule>
        {
            // Catalog: просмотр доступен всем
            new()
            {
                PathPrefix = "/catalog",
                Methods = new[] { "GET" },
                AllowAnonymous = true
            },

            // Catalog: изменение — только модератор контента и админ
            new()
            {
                PathPrefix = "/catalog",
                Methods = new[] { "POST", "PUT", "PATCH", "DELETE" },
                AllowedRoles = new[] { "ContentModerator", "Admin" }
            },

            // Orders: создание заказа — только авторизованные покупатель / оператор / админ
            new()
            {
                PathPrefix = "/orders/api/orders/parts",
                Methods = new[] { "POST" },
                AllowedRoles = new[] { "Customer", "Operator", "Admin" }
            },
            new()
            {
                PathPrefix = "/orders/api/orders/cars",
                Methods = new[] { "POST" },
                AllowedRoles = new[] { "Customer", "Operator", "Admin" }
            },

            // Orders: просмотр любых заказов — только оператор и админ
            new()
            {
                PathPrefix = "/orders/api/orders",
                Methods = new[] { "GET" },
                AllowedRoles = new[] { "Operator", "Admin" }
            },

            // Orders: удаление — только админ
            new()
            {
                PathPrefix = "/orders/api/orders",
                Methods = new[] { "DELETE" },
                AllowedRoles = new[] { "Admin" }
            }
        };

    /// <summary>
    /// Ищет правило доступа по пути и HTTP-методу.
    /// </summary>
    public static GatewayAccessRule? Find(string path, string method)
    {
        return GetRules()
            .OrderByDescending(x => x.PathPrefix.Length)
            .FirstOrDefault(rule =>
                path.StartsWith(rule.PathPrefix, StringComparison.OrdinalIgnoreCase) &&
                (rule.Methods.Length == 0 ||
                 rule.Methods.Contains(method, StringComparer.OrdinalIgnoreCase)));
    }
}
