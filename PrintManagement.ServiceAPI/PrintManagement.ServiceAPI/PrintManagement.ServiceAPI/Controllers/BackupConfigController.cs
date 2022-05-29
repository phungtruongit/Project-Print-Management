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
    public class BackupConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IBackupConfigRepository _backupConfigRepository;

        public BackupConfigController(PrintManagementContext context, ILogger logger, IBackupConfigRepository backupConfigRepository)
        {
            _context = context;
            _logger = logger;
            _backupConfigRepository = backupConfigRepository;
        }

        [HttpGet("main")]
        public async Task<ActionResult<ApiResultDTO<BackupConfig>>> GetBackupConfig() {
            try {
                var response = await _backupConfigRepository.GetMain();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetBackupConfig));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<BackupConfig>>>> GetAll()
        {
            try
            {
                var response = await _backupConfigRepository.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetAll));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("main")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(BackupConfigDTO request) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try {
                var res = await _backupConfigRepository.UpdateMain(request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<BackupConfig>>> GetById(Guid id)
        {
            try
            {
                var response = await _backupConfigRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, BackupConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid)
            {
                return BadRequest();
            }

            try
            {
                var res = await _backupConfigRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(BackupConfigDTO request)
        {
            try
            {
                var res = await _backupConfigRepository.InsertAsync(request);
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
                var response = await _backupConfigRepository.DeleteAsync(id);
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
