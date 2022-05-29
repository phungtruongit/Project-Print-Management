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
    public class WatermarkConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IWatermarkConfigRepository _watermarkConfigRepository;

        public WatermarkConfigController(PrintManagementContext context, ILogger logger, IWatermarkConfigRepository WatermarkConfigRepository)
        {
            _context = context;
            _logger = logger;
            _watermarkConfigRepository = WatermarkConfigRepository;
        }

        [HttpGet("ImageWatermark")]
        public async Task<ActionResult<ApiResultDTO<WatermarkConfig>>> GetImageWatermark()
        {
            try {
                var response = await _watermarkConfigRepository.GetImageWatermarkAsync();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetImageWatermark));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("TextWatermark")]
        public async Task<ActionResult<ApiResultDTO<WatermarkConfig>>> GetTextWatermark() {
            try {
                var response = await _watermarkConfigRepository.GetTextWatermarkAsync();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetTextWatermark));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("main")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(WatermarkConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try {
                var res = await _watermarkConfigRepository.UpdateWatermarkAsync(request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }
    }
}