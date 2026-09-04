using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

public class CatalogImageRequest
{
    [Required]
    [StringLength(500)]
    public string FileKey { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    [Url]
    public string Url { get; set; } = null!;

    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
