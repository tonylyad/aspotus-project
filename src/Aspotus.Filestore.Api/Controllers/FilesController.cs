using Aspotus.Filestore.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Aspotus.Filestore.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService= fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.GetAllAsync(cancellationToken);
                return Ok(result);
            });
        }

        [HttpGet("content")]
        public async Task<IActionResult> GetById([FromQuery] string key, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.DownloadFileAsync(key, cancellationToken);
                var contentType = ContentTypeProvider.TryGetContentType(key, out var detectedContentType)
                    ? detectedContentType
                    : "application/octet-stream";

                return File(result, contentType, enableRangeProcessing: true);
            });
        }

        [HttpPost("content")]
        [Consumes("application/octet-stream")]
        public async Task<IActionResult> Create([FromQuery] string key, [FromBody] byte[] content, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.UploadFileAsync(key, content, cancellationToken);
                return Ok(result);
            });
        }

        [HttpDelete("content")]
        public async Task<IActionResult> Delete([FromQuery] string key, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.DeleteFileAsync(key, cancellationToken);
                return StatusCode((int)result);
            });
        }

        private async Task<IActionResult> Execute(Func<Task<IActionResult>> func)
        {
            try
            {
                return await func();
            }
            catch (StatusCodeException x)
            {
                return StatusCode((int)x.StatusCode, x.Message);
            }
        }
    }
}
