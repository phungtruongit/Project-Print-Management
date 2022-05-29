using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.RepositoryServices;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase {
        private readonly PrintManagementContext _context;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;

        public UserController(PrintManagementContext context, ILogger logger,
            IUserRepository userRepository,
            IOptionsMonitor<AppSettings> optionsMonitor) {
            _context = context;
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultDTO<string>>> Login(LoginRequest loginModel) {
            try {
                var response = await _userRepository.LoginAsync(loginModel);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Login));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest) {
            try {
                var response = await _userRepository.RegisterAsync(registerRequest);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Register));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll() {
            try {
                var response = await _userRepository.GetAllAsync();
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
                var response = await _userRepository.GetAllPagingAsync(keyword, pageSize, pageIndex);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetAllPaging));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id) {
            try {
                var response = await _userRepository.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetById));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, UserDTO request) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Oid) {
                return BadRequest();
            }

            try {
                var res = await _userRepository.UpdateAsync(id, request);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, " - {0}", nameof(Update));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id) {
            try {
                var response = await _userRepository.DeleteAsync(id);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(Delete));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPatch("ResetUser")]
        public async Task<ActionResult> ResetUser(ResetUserRequest request) {
            try {
                var response = await _userRepository.ResetUserAsync(request);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(ResetUser));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request) {
            try {
                var response = await _userRepository.ChangePasswordAsync(request);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(ChangePassword));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("CheckPrintPermission")]
        public async Task<ActionResult> CheckPrintPermission(Guid id, int totalPage, string printerName) {
            try {
                var response = await _userRepository.CheckPrintPermissionAsync(id, totalPage, printerName);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(CheckPrintPermission));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("GetUserRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUserRole(Guid idUser) {
            try {
                var response = await _userRepository.GetUserRoleAsync(idUser);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetUserRole));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpGet("GetRoles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllRoles() {
            try {
                var response = await _userRepository.GetRolesAsync();
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(GetAllRoles));
                return BadRequest(ex.Envelope());
            }
        }

        [HttpPatch("RoleAssign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RoleAssign(RoleAssignRequest roleAssignRequest) {
            try {
                var response = await _userRepository.RoleAssignAsync(roleAssignRequest);
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - {0}", nameof(RoleAssign));
                return BadRequest(ex.Envelope());
            }
        }
    }
}
