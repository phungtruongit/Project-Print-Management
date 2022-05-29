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
    public class SystemConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly ISystemConfigOptionRepository _systemConfigOptionRepository;

        public SystemConfigController(PrintManagementContext context, ILogger logger, ISystemConfigOptionRepository systemConfigOptionRepository)
        {
            _context = context;
            _logger = logger;
            _systemConfigOptionRepository = systemConfigOptionRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<SystemConfigOption>>>> GetAll()
        {
            try
            {
                var response = await _systemConfigOptionRepository.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetAll));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<SystemConfigOption>>> GetById(Guid id)
        {
            try
            {
                var response = await _systemConfigOptionRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, SystemConfigOptionDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid)
            {
                return BadRequest();
            }

            try
            {
                var res = await _systemConfigOptionRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(SystemConfigOptionDTO request)
        {
            try
            {
                var res = await _systemConfigOptionRepository.InsertAsync(request);
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
                var response = await _systemConfigOptionRepository.DeleteAsync(id);
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
