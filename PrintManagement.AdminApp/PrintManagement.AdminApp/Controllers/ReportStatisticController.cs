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
using PrintManagement.AdminApp.Models;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;

namespace PrintManagement.AdminApp.Controllers {
    public class ReportStatisticController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly ISystemInfoApiClient _systemInfoApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IPrinterApiClient _printerApiClient;
        private readonly IDocumentApiClient _documentApiClient;
        private readonly IPrinterUsageLogApiClient _printerUsageLogApiClient;
        private readonly ILogger _logger;

        public ReportStatisticController(
            ISystemInfoApiClient systemInfoApiClient,
            IUserApiClient userApiClient,
            IPrinterApiClient printerApiClient,
            IDocumentApiClient documentApiClient,
            IPrinterUsageLogApiClient printerUsageLogApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _systemInfoApiClient = systemInfoApiClient;
            _userApiClient = userApiClient;
            _printerApiClient = printerApiClient;
            _documentApiClient = documentApiClient;
            _printerUsageLogApiClient = printerUsageLogApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index() {
            return View();
        }
        public async Task<IActionResult> ReportSystemOverview() {
            var data = (await _systemInfoApiClient.GetSystemInfoMain()).ResultObj;
            ViewData["systemInfo"] = data;
            return View();
        }
        public async Task<IActionResult> ReportUserStatistic() {
            var data = (await _userApiClient.GetAllUserAsync()).ResultObj;
            var flatData = new List<FlatUserDTO>();
            var token = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            var orderNumber = 0;
            foreach (var item in data) {
                flatData.Add(new FlatUserDTO() {
                    OrderNumber = String.Format("{0:n0}", (++orderNumber).ToString()),
                    UserName = item.UserName,
                    Name = item.Name,
                    Phone = item.Phone,
                    Email = item.Email,
                    Department = await CommonDataProvider.GetNameDepartmentByIdAsync(item.IdDepartment, token),
                    UserGroup = await CommonDataProvider.GetNameUserGroupByIdAsync(item.IdUserGroup, token),
                    Balance = String.Format("{0:n0}", item.Balance),
                    TotalJobs = String.Format("{0:n0}", item.TotalJobs),
                    TotalPages = String.Format("{0:n0}", item.TotalPages),
                    CreatedDate = @Convert.ToDateTime(item.CreatedDate).ToString("dd/MM/yyyy")
                });
            }
            return View(flatData);
        }
        public async Task<IActionResult> ReportPrinterStatistic() {
            var data = (await _printerApiClient.GetAllPrinter()).ResultObj;
            var flatData = new List<FlatPrinterDTO>();
            var token = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            var orderNumber = 0;
            foreach (var item in data) {
                flatData.Add(new FlatPrinterDTO() {
                    OrderNumber = String.Format("{0:n0}", (++orderNumber).ToString()),
                    Name = item.Name,
                    Location = item.Location,
                    Department = await CommonDataProvider.GetNameDepartmentByIdAsync(item.IdDepartment, token),
                    TotalJobs = String.Format("{0:n0}", item.TotalJobs),
                    TotalPages = String.Format("{0:n0}", item.TotalPages),
                    LastUsageDate = @Convert.ToDateTime(item.LastUsageDate).ToString("dd/MM/yyyy")
                });
            }
            return View(flatData);
        }
        public async Task<IActionResult> ReportDocumentStatistic() {
            var data = (await _documentApiClient.GetAllDocument()).ResultObj;
            var flatData = new List<FlatDocumentDTO>();
            var token = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            var orderNumber = 0;
            foreach (var item in data) {
                flatData.Add(new FlatDocumentDTO() {
                    OrderNumber = String.Format("{0:n0}", (++orderNumber).ToString()),
                    Name = item.Name,
                    MimeType = item.MimeType,
                    SizeKb = String.Format("{0:n0}", item.SizeKb),
                    CreatedBy = await CommonDataProvider.GetNameUserByIdAsync(item.IdUser, token),
                    CreatedDate = @Convert.ToDateTime(item.CreatedDate).ToString("dd/MM/yyyy")
                });
            }
            return View(flatData);
        }
        public async Task<IActionResult> ReportPrintTrafficStatistic() {
            var printUsageLogs = (await _printerUsageLogApiClient.GetAllPrinterUsageLog()).ResultObj;
            List<DataPoint> flatData = new List<DataPoint>();
            for (int i = 1; i <= 12; i++) {
                var printPageMonth = printUsageLogs.Where(x =>
                    Convert.ToDateTime(x.UsageDate).Year == DateTime.Today.Year
                    && Convert.ToDateTime(x.UsageDate).Month == i).Sum(x => x.TotalPages);
                flatData.Add(new DataPoint() {
                    OrderNumber = i.ToString(),
                    TimeItem = $"Tháng {i}/{DateTime.Now.Year}",
                    ContentItem = String.Format("{0:n0}", printPageMonth)
                });
            }
            return View(flatData);
        }
        public async Task<IActionResult> ReportPrintingExpensesStatistic() {
            var printUsageLogs = (await _printerUsageLogApiClient.GetAllPrinterUsageLog()).ResultObj;
            List<DataPoint> flatData = new List<DataPoint>();
            for (int i = 1; i <= 12; i++) {
                var printCostMonth = printUsageLogs.Where(x =>
                    Convert.ToDateTime(x.UsageDate).Year == DateTime.Today.Year
                    && Convert.ToDateTime(x.UsageDate).Month == i).Sum(x => x.UsageCost);
                flatData.Add(new DataPoint() {
                    OrderNumber = i.ToString(),
                    TimeItem = $"Tháng {i}/{DateTime.Now.Year}",
                    ContentItem = String.Format("{0:n0}", printCostMonth)
                });
            }
            return View(flatData);
        }
    }
}