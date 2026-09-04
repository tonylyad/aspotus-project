using Aspotus.Filestore.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Filestore.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
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

        [HttpGet("{**key}")]
        public async Task<IActionResult> GetById([FromRoute] string key, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.DownloadFileAsync(key, cancellationToken);
                return Ok(result);
            });
        }

        [HttpPost("{**key}")]
        [Consumes("application/octet-stream")]
        public async Task<IActionResult> Create([FromRoute] string key, [FromBody] byte[] content, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var result = await _fileService.UploadFileAsync(key, content, cancellationToken);
                return Ok(result);
            });
        }

        [HttpDelete("{**key}")]
        public async Task<IActionResult> Delete([FromRoute] string key, CancellationToken cancellationToken)
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
