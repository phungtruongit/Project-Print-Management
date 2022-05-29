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

namespace PrintManagement.ServiceAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(PrintManagementContext context, ILogger logger, IDocumentRepository documentRepository) {
            _context = context;
            _logger = logger;
            _documentRepository = documentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResultDTO<List<Document>>>> GetAll() {
            try {
                var response = await _documentRepository.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex) {
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
                    var res = await _documentRepository.GetAllPagingAsync(keyword, pageSize, pageIndex, userId);
                    return Ok(res);
                }
                var response = await _documentRepository.GetAllPagingAsync(keyword, pageSize, pageIndex);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetAllPaging));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResultDTO<Document>>> GetById(Guid id) {
            try {
                var response = await _documentRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Update(Guid id, DocumentDTO request) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid) {
                return BadRequest();
            }

            try {
                var res = await _documentRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResultDTO<bool>>> Insert(DocumentUploadRequest request) {
            try {
                var res = await _documentRepository.InsertAsync(request);
                return Ok(res);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Insert));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResultDTO<bool>>> Delete(Guid id) {
            try {
                var response = await _documentRepository.DeleteAsync(id);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Delete));
                return BadRequest(ex.Envelope());
            }
        }
    }
}
