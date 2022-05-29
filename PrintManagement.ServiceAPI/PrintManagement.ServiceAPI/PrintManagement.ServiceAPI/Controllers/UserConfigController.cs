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
    public class UserConfigController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IUserConfigRepository _userConfigRepository;

        public UserConfigController(PrintManagementContext context, ILogger logger, IUserConfigRepository UserConfigRepository)
        {
            _context = context;
            _logger = logger;
            _userConfigRepository = UserConfigRepository;
        }

        [HttpGet("main")]
        public async Task<ActionResult<ApiResultDTO<UserConfig>>> Get()
        {
            try
            {
                var response = await _userConfigRepository.GetMainAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Get));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("main")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(UserConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try {
                var res = await _userConfigRepository.UpdateMainAsync(request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }
    }
}
