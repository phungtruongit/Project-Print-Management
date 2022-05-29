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
    public class DepartmentController : ControllerBase
    {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(PrintManagementContext context, ILogger logger, IDepartmentRepository departmentRepository)
        {
            _context = context;
            _logger = logger;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<Department>>>> GetAll()
        {
            try
            {
                var response = await _departmentRepository.GetAllAsync();
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
                var response = await _departmentRepository.GetAllPagingAsync(keyword, pageSize, pageIndex);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetAllPaging));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<Department>>> GetById(Guid id)
        {
            try
            {
                var response = await _departmentRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, DepartmentDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid)
            {
                return BadRequest();
            }

            try
            {
                var res = await _departmentRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(DepartmentDTO request)
        {
            try
            {
                var res = await _departmentRepository.InsertAsync(request);
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
                var response = await _departmentRepository.DeleteAsync(id);
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
