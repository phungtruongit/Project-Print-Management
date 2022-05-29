using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PrintManagement.Infrastructure;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace PrintManagement.ServiceAPI.Controllers {
    [Route("api")]
    public class ApiController : ControllerBase {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        public ApiController(PrintManagementContext context, ILogger logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("on")]
        public ActionResult on() {
            _logger.LogTrace("Running...");
            return Ok("Running...");
        }

        [HttpGet]
        [Route("ondb")]
        public ActionResult dbOn() {
            var dbConnected = _context != null && _context.Database.GetDbConnection().ConnectionString != null;
            _logger.LogTrace($"Running... DB connected: {dbConnected}");
            return Ok($"Running... DB connected: {dbConnected}");
        }

        [HttpGet]
        [Route("onerror")]
        public ActionResult onError() => throw new System.Exception("Error test");

        [HttpGet]
        [Route("onnocontent")]
        public ActionResult onNoContent() => NoContent();
    }
}
