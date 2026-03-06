using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                service = "Aspotus.Gateway"
            });
        }
    }
}
