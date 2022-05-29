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
    public class PrintUsageLogController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IPrinterUsageLogApiClient _printerUserLogApiClient;
        private readonly ILogger _logger;

        public PrintUsageLogController(IPrinterUsageLogApiClient printerUsageLogApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _printerUserLogApiClient = printerUsageLogApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index(string keyword = "empty", int pageIndex = 1, int pageSize = 10) {
            if (string.IsNullOrEmpty(keyword))
                keyword = "empty";

            var data = await _printerUserLogApiClient.GetAllPrinterUsageLog(keyword, pageSize, pageIndex);

            if (!keyword.Equals("empty"))
                ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];

            ViewData["Token"] = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);

            return View(data.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id) {
            var result = await _printerUserLogApiClient.GetPrinterUsageLogById(id);
            //var token = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            //ViewData["userName"] = await CommonDataProvider.GetNameUserByIdAsync(result.ResultObj.IdUser, token);
            //ViewData["printerName"] = await CommonDataProvider.GetNamePrinterByIdAsync(result.ResultObj.IdPrinter, token);
            //ViewData["documentName"] = await CommonDataProvider.GetNameDocumentByIdAsync(result.ResultObj.IdDocument, token);
            return View(result.ResultObj);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DocumentViewer(Guid idPrintUsageLog, Guid idDocument) {
            ViewData["Token"] = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            ViewData["idPrintUsageLog"] = idPrintUsageLog;
            ViewData["idDocument"] = idDocument;

            // TODO

            return View();
        }
    }
}