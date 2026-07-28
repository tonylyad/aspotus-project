namespace Aspotus.Catalog.Api.Options;

/// <summary>
/// Настройки кэширования марок автомобилей.
/// </summary>
public sealed class BrandCacheOptions
{
    /// <summary>
    /// Название секции конфигурации.
    /// </summary>
    public const string SectionName = "Cache";

    /// <summary>
    /// Время хранения марок автомобилей в распределённом кэше.
    /// </summary>
    public int BrandsExpirationMinutes { get; set; } = 10;
}
