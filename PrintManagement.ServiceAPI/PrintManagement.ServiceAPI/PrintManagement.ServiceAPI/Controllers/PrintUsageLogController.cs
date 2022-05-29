using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.RepositoryServices;
using PrintManagement.ServiceAPI.Services;
using System.Security.Claims;
using VCSLib.HMAC;

namespace PrintManagement.ServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrintUsageLogController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IPrintUsageLogRepository _printUsageLogRepository;
        private readonly AppSettings _appSettings;
        private readonly IHashCodeHMAC _hashCodeHMAC;

        public PrintUsageLogController(PrintManagementContext context, ILogger logger, IHashCodeHMAC hashCodeHMAC,
            IPrintUsageLogRepository printUsageLogRepository, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _context = context;
            _logger = logger;
            _printUsageLogRepository = printUsageLogRepository;
            _appSettings = optionsMonitor.CurrentValue;
            _hashCodeHMAC = hashCodeHMAC;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<PrinterUsageLog>>>> GetAll()
        {
            try
            {
                var response = await _printUsageLogRepository.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetAll));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("paging")]
        public async Task<ActionResult> GetAllPaging([FromQuery] string keyword, int pageSize, int pageIndex) {
            try {
                var isAdmin = User.Claims.Any(x => x.Type.Equals(ClaimTypes.Role) && x.Value.Equals("Admin"));
                if (!isAdmin) {
                    var userId = User.Claims.First(x => x.Type.Equals("UserId")).Value;
                    var res = await _printUsageLogRepository.GetAllPagingAsync(keyword, pageSize, pageIndex, userId);
                    return Ok(res);
                }
                var response = await _printUsageLogRepository.GetAllPagingAsync(keyword, pageSize, pageIndex);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetAllPaging));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<PrinterUsageLog>>> GetById(Guid id)
        {
            try
            {
                var response = await _printUsageLogRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, PrinterUsageLogDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid)
            {
                return BadRequest();
            }

            try
            {
                var res = await _printUsageLogRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        [Authorize]
        [HMACAuthorize]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(PrinterUsageLogDTO request)
        {
            try
            {
                var bodyContent = await HMACAuthorizeAttribute.GetRawBodyAsync(HttpContext.Request);
                var userId = User.Claims.ElementAt(4).Value;
                var requestContentStringBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(bodyContent));
                var signature = _hashCodeHMAC.ComputeSignatureHMACSHA256(requestContentStringBase64,userId,request.MachineName);

                request.Signature = signature;

                var res = await _printUsageLogRepository.InsertAsync(request);
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
                var response = await _printUsageLogRepository.DeleteAsync(id);
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
