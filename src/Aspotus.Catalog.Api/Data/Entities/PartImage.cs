namespace Aspotus.Catalog.Api.Data.Entities;

public class PartImage
{
    public Guid Id { get; set; }
    public Guid PartId { get; set; }
    public Part Part { get; set; } = null!;
    public string FileKey { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
