using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;

namespace PrintManagement.AdminApp.Controllers {
    public class AccountController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly ILogger _logger;

        public AccountController(IUserApiClient systemInfoApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _userApiClient = systemInfoApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index() {
            // TODO
            var strId = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId"))?.Value;
            if (strId != null) {
                var userId = Guid.Parse(strId);
                var data = await _userApiClient.GetByIdAsync(userId);

                var usergroupName = await _baseApiClient
                    .GetAsync<ApiResultDTO<UserGroupDTO>>($"/api/usergroup/{data.ResultObj.IdUserGroup}");
                var departmentName = await _baseApiClient
                    .GetAsync<ApiResultDTO<DepartmentDTO>>($"/api/department/{data.ResultObj.IdDepartment}");

                ViewData["usergroupName"] = usergroupName.ResultObj.Name;
                ViewData["departmentName"] = departmentName.ResultObj.Name;

                return View(data.ResultObj);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(Guid id) {
            var model = new ChangePasswordRequest() {
                Oid = id,
                ModifiedBy = User.Claims.FirstOrDefault()?.Value,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request) {
            var result = await _userApiClient.ChangePasswordAsync(request);
            if (!result.IsSuccessed) {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            TempData["result"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Index");
        }
    }
}