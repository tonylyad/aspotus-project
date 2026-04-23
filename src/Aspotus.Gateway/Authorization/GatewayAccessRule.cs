namespace Aspotus.Gateway.Authorization;

/// <summary>
/// Правило доступа к проксируемому маршруту Gateway.
/// </summary>
public class GatewayAccessRule
{
    /// <summary>
    /// Префикс пути, к которому применяется правило.
    /// </summary>
    public string PathPrefix { get; set; } = string.Empty;

    /// <summary>
    /// HTTP-методы, к которым применяется правило.
    /// </summary>
    public string[] Methods { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Разрешён ли анонимный доступ.
    /// </summary>
    public bool AllowAnonymous { get; set; }

    /// <summary>
    /// Роли, которым разрешён доступ.
    /// </summary>
    public string[] AllowedRoles { get; set; } = Array.Empty<string>();
}