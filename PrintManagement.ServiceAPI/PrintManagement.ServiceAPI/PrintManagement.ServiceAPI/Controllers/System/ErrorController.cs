using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PrintManagement.ServiceAPI.Controllers {
    [ApiController]
    [Route("api")]
    public class ErrorController : ControllerBase {
        private readonly ILogger _logger;
        public ErrorController(ILogger logger) {
            _logger = logger;
        }

        [HttpGet("error")]
        public IActionResult Error() {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            _logger.LogError(context?.Error, "ErrorHandler");
            return StatusCode(StatusCodes.Status500InternalServerError, context?.Error);
        }
    }
}
