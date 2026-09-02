namespace Aspotus.Filestore.Api.Infrastructure
{
    /// <summary>
    /// Информация о файле во внешнем хранилище.
    /// </summary>
    public record FileItem
    {
        public required string Key { get; set; }
        public required long Size { get; set; }
        public required string Url { get; set; }
    }
}
