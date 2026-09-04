namespace Aspotus.Catalog.Api.Data.Entities;

public class CarImage
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Car Car { get; set; } = null!;
    public string FileKey { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
