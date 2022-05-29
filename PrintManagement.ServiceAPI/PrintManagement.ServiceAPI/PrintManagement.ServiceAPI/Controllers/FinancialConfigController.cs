using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.RepositoryServices;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FinancialConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IFinancialConfigRepository _financialConfigRepository;

        public FinancialConfigController(PrintManagementContext context, ILogger logger, IFinancialConfigRepository FinancialConfigRepository)
        {
            _context = context;
            _logger = logger;
            _financialConfigRepository = FinancialConfigRepository;
        }

        [HttpGet("main")]
        public async Task<ActionResult<ApiResultDTO<FinancialConfig>>> Get()
        {
            try {
                var response = await _financialConfigRepository.GetMainAsync();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Get));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("main")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(FinancialConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try {
                var res = await _financialConfigRepository.UpdateMainAsync(request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }
    }
}
