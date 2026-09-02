using System.Net;

namespace Aspotus.Filestore.Api.Infrastructure
{
    public interface IFileService
    {
        Task<List<FileItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<FileItem> UploadFileAsync(string key, byte[] content, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadFileAsync(string key, CancellationToken cancellationToken = default);
        Task<HttpStatusCode> DeleteFileAsync(string key, CancellationToken cancellationToken = default);
    }
}
