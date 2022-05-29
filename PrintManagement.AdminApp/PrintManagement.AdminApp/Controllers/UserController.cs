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
using PrintManagement.MailHub.EmailProvider;

namespace PrintManagement.AdminApp.Controllers {
    public class UserController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IEmailSender _emailSender;
        //private readonly IRoleApiClient _roleApiClient;
        private readonly ILogger _logger;

        public UserController(IUserApiClient userApiClient, IEmailSender emailSender,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _userApiClient = userApiClient;
            _emailSender = emailSender;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index(string keyword = "empty", int pageIndex = 1, int pageSize = 10) {
            if (string.IsNullOrEmpty(keyword))
                keyword = "empty";

            var data = await _userApiClient.GetUsersPagingsAsync(keyword, pageSize, pageIndex);

            if (!keyword.Equals("empty"))
                ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];

            ViewData["Token"] = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);

            return View(data.ResultObj);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id) {
            var result = await _userApiClient.GetByIdAsync(id);

            var usergroupName = await _baseApiClient
                .GetAsync<ApiResultDTO<UserGroupDTO>>($"/api/usergroup/{result.ResultObj.IdUserGroup}");
            var departmentName = await _baseApiClient
                .GetAsync<ApiResultDTO<DepartmentDTO>>($"/api/department/{result.ResultObj.IdDepartment}");

            ViewData["usergroupName"] = usergroupName.ResultObj.Name;
            ViewData["departmentName"] = departmentName.ResultObj.Name;

            return View(result.ResultObj);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create() {
            var userGroups = await _baseApiClient.GetAsync<ApiResultDTO<List<UserGroupDTO>>>("/api/usergroup");
            var departments = await _baseApiClient.GetAsync<ApiResultDTO<List<DepartmentDTO>>>("/api/department");

            var selectListUserGroups = userGroups.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));
            var selectListDepartments = departments.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));

            ViewData["selectListUserGroups"] = selectListUserGroups;
            ViewData["selectListDepartments"] = selectListDepartments;


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RegisterRequest request) {
            if (!ModelState.IsValid)
                return View();

            request.IsAdmin = false;
            request.Note = String.Empty;
            request.RemainPages = 1000;
            request.CreatedBy = User.Claims.FirstOrDefault()?.Value;
            request.CreatedDate = DateTime.Now;
            request.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            request.ModifiedDate = DateTime.Now;

            if (request.IdUserGroup.HasValue)
                if (request.IdUserGroup.Equals(new Guid("3ee60cfa-3e58-48ed-8410-2a1effd412a3"))) // Quản trị viên
                    request.IsAdmin = true;

            var result = await _userApiClient.RegisterUserAsync(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Thêm mới người dùng thành công";
                Common.CommonBussiness.SendMailNewUser(_emailSender, request.Email, request.Name, request.UserName, request.Password);
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id) {
            var userGroups = await _baseApiClient.GetAsync<ApiResultDTO<List<UserGroupDTO>>>("/api/usergroup");
            var departments = await _baseApiClient.GetAsync<ApiResultDTO<List<DepartmentDTO>>>("/api/department");

            var selectListUserGroups = userGroups.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));
            var selectListDepartments = departments.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));

            ViewData["selectListUserGroups"] = selectListUserGroups;
            ViewData["selectListDepartments"] = selectListDepartments;

            var result = await _userApiClient.GetByIdAsync(id);
            if (result.IsSuccessed) {
                return View(result.ResultObj);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UserDTO request) {
            if (!ModelState.IsValid)
                return View();
            if (request.IdUserGroup.HasValue)
                if (request.IdUserGroup.Equals(new Guid("3ee60cfa-3e58-48ed-8410-2a1effd412a3")))
                    request.IsAdmin = true;

            request.IsAdmin = false;
            request.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            request.ModifiedDate = DateTime.Now;

            var result = await _userApiClient.UpdateUserAsync(request.Oid, request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật người dùng thành công";
                return RedirectToAction("Index");
            }

            var userGroups = await _baseApiClient.GetAsync<ApiResultDTO<List<UserGroupDTO>>>("/api/usergroup");
            var departments = await _baseApiClient.GetAsync<ApiResultDTO<List<DepartmentDTO>>>("/api/department");

            var selectListUserGroups = userGroups.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));
            var selectListDepartments = departments.ResultObj.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));

            ViewData["selectListUserGroups"] = selectListUserGroups;
            ViewData["selectListDepartments"] = selectListDepartments;

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove(SystemConstants.AppSetting.Token);
            return RedirectToAction("Index", "Login");
        }

        //[HttpGet]
        //public IActionResult Delete(Guid id)
        //{
        //    return View(new UserDeleteRequest()
        //    {
        //        Id = id
        //    });
        //}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id) {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.DeleteAsync(id);
            if (result.IsSuccessed) {
                TempData["result"] = "Xóa người dùng thành công";
            }
            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetUser(Guid id) {
            if (!ModelState.IsValid)
                return View();

            var contentRequest = new ResetUserRequest {
                Oid = id,
                ResetBy = User.Claims.FirstOrDefault()?.Value,
                ModifiedBy = User.Claims.FirstOrDefault()?.Value
            };

            var result = await _userApiClient.ResetUserAsync(contentRequest);
            if (result.IsSuccessed) {
                TempData["result"] = "Reset dùng thành công";
            }
            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> RoleAssign(Guid id, string name) {
            var idRole = (await _userApiClient.GetUserRoleAsync(id)).ResultObj;
            var roles = (await _userApiClient.GetRolesAsync()).ResultObj;
            var selectListRoles = roles.Select(x => new SelectListItem(x.Name, x.Oid.ToString()));

            ViewData["selectListRoles"] = selectListRoles;

            var roleAssignRequest = new RoleAssignRequest {
                IdRole = idRole,
                IdUser = id,
                Name = name,
            };
            return View(roleAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(RoleAssignRequest request) {
            if (!ModelState.IsValid)
                return View();

            request.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            request.ModifiedDate = DateTime.Now;

            var result = await _userApiClient.RoleAssignAsync(request);

            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật quyền thành công";
                return RedirectToAction("Index");
            }

            return View(request);
        }

        //private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid id)
        //{
        //    var userObj = await _userApiClient.GetById(id);
        //    var roleObj = await _roleApiClient.GetAll();
        //    var roleAssignRequest = new RoleAssignRequest();
        //    foreach (var role in roleObj.ResultObj)
        //    {
        //        roleAssignRequest.Roles.Add(new SelectItem()
        //        {
        //            Id = role.Id.ToString(),
        //            Name = role.Name,
        //            Selected = userObj.ResultObj.Roles.Contains(role.Name)
        //        });
        //    }
        //    return roleAssignRequest;
        //}

        //public async Task<string> GetNameUserGroupByIdAsync(Guid id) {
        //    var usergroupName = await _baseApiClient
        //        .GetAsync<ApiResultDTO<UserGroupDTO>>($"/api/usergroup/{id}");
        //    return usergroupName.ResultObj.Name;
        //}

        //public async Task<string> GetNameDepartmentByIdAsync(Guid id) {
        //    var departmentName = await _baseApiClient
        //        .GetAsync<ApiResultDTO<DepartmentDTO>>($"/api/department/{id}");

        //    return departmentName.ResultObj.Name;
        //}
        public async Task<IActionResult> Forbidden() {
            return View();
        }

        public async Task<IActionResult> Error() {
            return View();
        }
    }
}
