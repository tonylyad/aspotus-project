namespace Aspotus.Catalog.Api.Models.Responses;

public class CatalogImageResponse
{
    public Guid Id { get; set; }
    public string FileKey { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
