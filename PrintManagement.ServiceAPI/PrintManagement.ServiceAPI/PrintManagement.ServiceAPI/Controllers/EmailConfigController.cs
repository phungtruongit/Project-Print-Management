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
    public class EmailConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IEmailConfigRepository _emailConfigRepository;

        public EmailConfigController(PrintManagementContext context, ILogger logger, IEmailConfigRepository emailConfigRepository)
        {
            _context = context;
            _logger = logger;
            _emailConfigRepository = emailConfigRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<EmailConfig>>>> GetAll()
        {
            try
            {
                var response = await _emailConfigRepository.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetAll));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("main")]
        public async Task<ActionResult<ApiResultDTO<EmailConfig>>> GetEmailConfig() {
            try {
                var response = await _emailConfigRepository.GetMain();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetEmailConfig));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<EmailConfig>>> GetById(Guid id)
        {
            try
            {
                var response = await _emailConfigRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("main")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(EmailConfigDTO request) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try {
                var res = await _emailConfigRepository.UpdateMain(request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, EmailConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid)
            {
                return BadRequest();
            }

            try
            {
                var res = await _emailConfigRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(EmailConfigDTO request)
        {
            try
            {
                var res = await _emailConfigRepository.InsertAsync(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Insert));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Delete(Guid id)
        {
            try
            {
                var response = await _emailConfigRepository.DeleteAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Delete));
                return BadRequest(ex.Envelope());
            }
        }
    }
}
