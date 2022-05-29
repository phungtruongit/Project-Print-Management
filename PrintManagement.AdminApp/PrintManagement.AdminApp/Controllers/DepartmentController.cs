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
    public class DepartmentController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IDepartmentApiClient _departmentApiClient;
        private readonly ILogger _logger;

        public DepartmentController(IDepartmentApiClient departmentApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _departmentApiClient = departmentApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index(string keyword = "empty", int pageIndex = 1, int pageSize = 10) {
            if (string.IsNullOrEmpty(keyword))
                keyword = "empty";

            var data = await _departmentApiClient.GetAllDepartment(keyword, pageSize, pageIndex);

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
            var result = await _departmentApiClient.GetDepartmentById(id);
            return View(result.ResultObj);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create() {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DepartmentDTO department) {
            if (!ModelState.IsValid)
                return View();

            department.CreatedBy = User.Claims.FirstOrDefault()?.Value;
            department.CreatedDate = DateTime.Now;
            department.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            department.ModifiedDate = DateTime.Now;

            var result = await _departmentApiClient.CreateDepartment(department);
            if (result.IsSuccessed) {
                TempData["result"] = "Thêm mới phòng ban thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(department);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id) {
            var result = await _departmentApiClient.GetDepartmentById(id);
            if (result.IsSuccessed) {
                return View(result.ResultObj);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(DepartmentDTO request) {
            if (!ModelState.IsValid)
                return View();

            request.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            request.ModifiedDate = DateTime.Now;

            var result = await _departmentApiClient.UpdateDepartment(request.Oid, request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật phòng ban thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id) {
            if (!ModelState.IsValid)
                return View();

            var result = await _departmentApiClient.DeleteDepartment(id);
            if (result.IsSuccessed) {
                TempData["result"] = "Xóa phòng ban thành công";
            }
            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index");
        }
    }
}