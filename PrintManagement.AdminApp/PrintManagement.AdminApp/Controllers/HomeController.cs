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
using Newtonsoft.Json;
using PrintManagement.AdminApp.Models;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;
using PrintManagement.EmailHub.Models;
using PrintManagement.MailHub.EmailProvider;

namespace PrintManagement.AdminApp.Controllers {
    public class HomeController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly ISystemInfoApiClient _systemInfoApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IPrinterApiClient _printerApiClient;
        private readonly IPrinterUsageLogApiClient _printerUsageLogInfoApiClient;
        private readonly IUserConfigApiClient _userConfigApiClient;
        private readonly IEmailConfigApiClient _emailConfigApiClient;
        private readonly IFinancialConfigApiClient _financialApiClient;
        private readonly IBackupConfigApiClient _backupConfigApiClient;
        private readonly IWatermarkConfigApiClient _watermarkConfigApiClient;
        private readonly ILogger _logger;

        public HomeController(ISystemInfoApiClient systemInfoApiClient,
            IUserApiClient userApiClient,
            IPrinterApiClient printerApiClient,
            IPrinterUsageLogApiClient printerUsageLogApiClient,
            IUserConfigApiClient userConfigApiClient,
            IEmailConfigApiClient emailConfigApiClient,
            IFinancialConfigApiClient financialConfigApiClient,
            IBackupConfigApiClient backupConfigApiClient,
            IWatermarkConfigApiClient watermarkConfigApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _systemInfoApiClient = systemInfoApiClient;
            _userApiClient = userApiClient;
            _printerApiClient = printerApiClient;
            _printerUsageLogInfoApiClient = printerUsageLogApiClient;
            _userConfigApiClient = userConfigApiClient;
            _emailConfigApiClient = emailConfigApiClient;
            _backupConfigApiClient = backupConfigApiClient;
            _financialApiClient = financialConfigApiClient;
            _watermarkConfigApiClient = watermarkConfigApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index() {
            ViewData["Token"] = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            //var systemInfoMain = (await _systemInfoApiClient.GetSystemInfoMain()).ResultObj;
            var printer = await _printerApiClient.GetAllPrinter();  
            var users = (await _userApiClient.GetAllUserAsync()).ResultObj;
            var dataTopUsageUser = users.OrderByDescending(x => x.TotalPages).Take(5).ToList();
            var dataTopUsagePrinter = (await _printerApiClient.GetAllPrinter()).ResultObj.OrderByDescending(x => x.TotalPages).Take(5).ToList();
            var printUsageLogs = (await _printerUsageLogInfoApiClient.GetAllPrinterUsageLog()).ResultObj;
            var dataTopNewPrintUsageLog = printUsageLogs.OrderByDescending(x => x.UsageDate).Take(5).ToList();
            var dataTopNewUser = (await _userApiClient.GetAllUserAsync()).ResultObj.OrderByDescending(x => x.CreatedDate).Take(8).ToList();
            
            // System Info
            ViewData["totalUsers"] = users.Count;
            ViewData["totalPrinters"] = printer.ResultObj.Count;
            ViewData["totalJobs"] = printUsageLogs.Where(x => x.IsCancelled.Equals(false)).ToList().Count;
            ViewData["totalPrintCosts"] = printUsageLogs.Sum(x => x.UsageCost);

            // Chart 2: Top Usage User
            dataTopUsageUser.Reverse();
            var dataPointTopUsageUser = new List<DataPoint>();
            foreach (var item in dataTopUsageUser)
                dataPointTopUsageUser.Add(new DataPoint(item.Name, Convert.ToDouble(item.TotalPages)));

            // Chart 3: Top Usage Printer
            var dataPointTopUsagePrinter = new List<DataPoint>();
            foreach (var item in dataTopUsagePrinter)
                dataPointTopUsagePrinter.Add(new DataPoint(item.Name, Convert.ToDouble(item.TotalPages)));

            // Chart 4: Print Traffic
            List<DataPoint> dataPointsTraffic = new List<DataPoint>();
            for (int i = 1; i <= 12; i++) {
                var printPageMonth = printUsageLogs.Where(x =>
                    Convert.ToDateTime(x.UsageDate).Year == DateTime.Today.Year
                    && Convert.ToDateTime(x.UsageDate).Month == i).Sum(x => x.TotalPages);
                dataPointsTraffic.Add(new DataPoint($"Tháng {i}", Convert.ToDouble(printPageMonth)));
            }

            // Chart 5: Print Expenses
            List<DataPoint> dataPointsPrintExpenses = new List<DataPoint>();
            for (int i = 1; i <= 12; i++) {
                var printCostMonth = printUsageLogs.Where(x => 
                    Convert.ToDateTime(x.UsageDate).Year == DateTime.Today.Year 
                    && Convert.ToDateTime(x.UsageDate).Month == i).Sum(x => x.UsageCost);
                dataPointsPrintExpenses.Add(new DataPoint($"Tháng {i}", Convert.ToDouble(printCostMonth)));
            }

            // Set ViewBag
            ViewBag.DataPointsTopUsageUser = JsonConvert.SerializeObject(dataPointTopUsageUser);
            ViewBag.DataPointsTopUsagePrinter = JsonConvert.SerializeObject(dataPointTopUsagePrinter);
            ViewBag.DataPointsPrintTraffic = JsonConvert.SerializeObject(dataPointsTraffic);
            ViewBag.DataPointsPrintExpenses = JsonConvert.SerializeObject(dataPointsPrintExpenses);
            ViewData["lstNewLog"] = dataTopNewPrintUsageLog;
            ViewData["lstNewUser"] = dataTopNewUser;

            // Get common config
            await Common.CommonBussiness.GetCommonConfig(_userConfigApiClient, _emailConfigApiClient, _backupConfigApiClient, _financialApiClient, _watermarkConfigApiClient);

            return View();
        }
    }
}