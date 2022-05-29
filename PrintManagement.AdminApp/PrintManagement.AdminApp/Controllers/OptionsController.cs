using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Watermark;
using GroupDocs.Watermark.Watermarks;
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
    public class OptionsController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IUserApiClient _userApiclient;
        private readonly IEmailConfigApiClient _emailConfigApiClient;
        private readonly IBackupConfigApiClient _backupConfigApiClient;
        private readonly IUserConfigApiClient _userConfigApiClient;
        private readonly IFinancialConfigApiClient _financialConfigApiClient;
        private readonly IWatermarkConfigApiClient _watermarkConfigApiClient;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public OptionsController(IUserApiClient userApiClient,
            IEmailConfigApiClient emailConfigApiClient,
            IBackupConfigApiClient backupConfigApiClient,
            IUserConfigApiClient userConfigApiClient,
            IFinancialConfigApiClient financialConfigApiClient,
            IWatermarkConfigApiClient watermarkConfigApiClient,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _userApiclient = userApiClient;
            _emailConfigApiClient = emailConfigApiClient;
            _backupConfigApiClient = backupConfigApiClient;
            _userConfigApiClient = userConfigApiClient;
            _financialConfigApiClient = financialConfigApiClient;
            _watermarkConfigApiClient = watermarkConfigApiClient;
            _emailSender = emailSender;
            _baseApiClient = baseApiClient;
            _env = webHostEnvironment;
        }

        public async Task<IActionResult> Index() {
            var userConfig = (await _userConfigApiClient.GetUserConfig()).ResultObj;
            var emailConfig = (await _emailConfigApiClient.GetEmailConfig()).ResultObj;
            var backupConfig = (await _backupConfigApiClient.GetBackupConfig()).ResultObj;
            var financialConfig = (await _financialConfigApiClient.GetFinancialConfig()).ResultObj;
            var imageWatermarkConfig = (await _watermarkConfigApiClient.GetImageWatermarkConfig()).ResultObj;
            var textWatermarkConfig = (await _watermarkConfigApiClient.GetTextWatermarkConfig()).ResultObj;
            //var advancedConfig = ...

            var tupleWatermark =

            ViewData["UserConfig"] = userConfig;
            ViewData["EmailConfig"] = emailConfig;
            ViewData["BackupConfig"] = backupConfig;
            ViewData["FinancialConfig"] = financialConfig;
            ViewData["WatermarkConfig"] = new Tuple<WatermarkConfigDTO, WatermarkConfigDTO>(imageWatermarkConfig, textWatermarkConfig);
            //ViewData["AdvancedConfig"] = advancedConfig;

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyAdvancedConfig(BackupConfigDTO request) {
            // TODO
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyBackupConfig(BackupConfigDTO request) {
            if (!ModelState.IsValid)
                return View();

            var result = await _backupConfigApiClient.UpdateBackupConfig(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật cấu hình sao dữ liệu thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyEmailConfig(EmailConfigDTO request) {
            if (!ModelState.IsValid)
                return View();

            var result = await _emailConfigApiClient.UpdateEmailConfig(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật cấu hình email thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyFinancialConfig(FinancialConfigDTO request) {
            if (!ModelState.IsValid)
                return View();

            var result = await _financialConfigApiClient.UpdateFinancialConfig(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật cấu hình tài chính thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyUserConfig(UserConfigDTO request) {
            //if (!ModelState.IsValid)
            //    return View();

            var result = await _userConfigApiClient.UpdateUserConfig(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật cấu hình người dùng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyImageWatermarkConfig(IFormFile file, WatermarkConfigDTO request) {

            request.FileImage = file.FileName;

            if (!ModelState.IsValid)
                return View();

            var result = await _watermarkConfigApiClient.UpdateWatermarkConfig(request);
            if (result.IsSuccessed) {
                var uploadsFolder = Path.Combine(_env.WebRootPath, @"template\Admin\ImgWatermark");
                var filePath = Path.Combine(uploadsFolder, file.FileName);
                if (System.IO.File.Exists(filePath)) {
                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                TempData["result"] = "Cập nhật cấu hình watermark hình thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApplyTextWatermarkConfig(WatermarkConfigDTO request) {
            if (!ModelState.IsValid)
                return View();

            var result = await _watermarkConfigApiClient.UpdateWatermarkConfig(request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật cấu hình watermark chữ thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index", "Options");
        }

        public async Task<IActionResult> TestMail(string recipientAddress) {
            Common.CommonBussiness.TestMailSMTP(_emailSender, recipientAddress);
            return RedirectToAction("Index", "Options");
        }

        public async Task<IActionResult> BackupNow() {
            Common.CommonBussiness.BackupDatabase();
            return RedirectToAction("Index", "Options");
        }

        public async Task<IActionResult> RestoreNow(string fileBackup) {
            Common.CommonBussiness.RestoreDatabase(fileBackup);
            return RedirectToAction("Index", "Options");
        }

        public async Task<IActionResult> TestImageWatermark(string fileImage, double? x, double? y, int? verticalAlignment, int? horizontalAlignment, double? height, double? width, double? rotateAngle, bool? isBackground, double? opacity) {
            var imageWatermark = new ImageWatermark(Path.Combine(_env.WebRootPath, @$"template\Admin\ImgWatermark\none.png"));
            var pathImage = Path.Combine(_env.WebRootPath, @$"template\Admin\ImgWatermark\{fileImage}");
            if (System.IO.File.Exists(pathImage)) {
                imageWatermark = new ImageWatermark(Path.Combine(_env.WebRootPath, pathImage));
            }
            if (x!=null)
                imageWatermark.X = Convert.ToDouble(x);
            if (y!=null)
                imageWatermark.Y = Convert.ToDouble(y);
            if (verticalAlignment!=null)
                imageWatermark.VerticalAlignment = (GroupDocs.Watermark.Common.VerticalAlignment)Convert.ToInt32(verticalAlignment);
            if (horizontalAlignment!=null)
                imageWatermark.HorizontalAlignment = (GroupDocs.Watermark.Common.HorizontalAlignment)Convert.ToInt32(horizontalAlignment);
            if (height!=null)
                imageWatermark.Height = Convert.ToDouble(height);
            if (width!=null)
                imageWatermark.Width = Convert.ToDouble(width);
            if (rotateAngle!=null)
                imageWatermark.RotateAngle = Convert.ToDouble(rotateAngle);
            if (isBackground!=null)
                imageWatermark.IsBackground = Convert.ToBoolean(isBackground);
            if (opacity!=null)
                imageWatermark.Opacity = Convert.ToDouble(opacity);

            var input = Path.Combine(_env.WebRootPath, @$"template\Admin\Docs\TestDocument.pdf");
            var output = Path.Combine(_env.WebRootPath, @$"template\Admin\Docs\TestImageWatermark.pdf");
            WatermarkHub.WatermarkProvider.AddImageWatermarkToFile(imageWatermark, input, output);

            return View("TestImageWatermark", "Options");
        }

        public async Task<IActionResult> TestTextWatermark(string text, int fontSize, double? x, double? y, int? verticalAlignment, int? horizontalAlignment, double? height, double? width, double? rotateAngle, bool? isBackground, double? opacity) {
            var textWatermark = new TextWatermark(text, new Font("Arial", fontSize));
            if (x!=null)
                textWatermark.X = Convert.ToDouble(x);
            if (y!=null)
                textWatermark.Y = Convert.ToDouble(y);
            if (verticalAlignment!=null)
                textWatermark.VerticalAlignment = (GroupDocs.Watermark.Common.VerticalAlignment)Convert.ToInt32(verticalAlignment);
            if (horizontalAlignment!=null)
                textWatermark.HorizontalAlignment = (GroupDocs.Watermark.Common.HorizontalAlignment)Convert.ToInt32(horizontalAlignment);
            if (height!=null)
                textWatermark.Height = Convert.ToDouble(height);
            if (width!=null)
                textWatermark.Width = Convert.ToDouble(width);
            if (rotateAngle!=null)
                textWatermark.RotateAngle = Convert.ToDouble(rotateAngle);
            if (isBackground!=null)
                textWatermark.IsBackground = Convert.ToBoolean(isBackground);
            if (opacity!=null)
                textWatermark.Opacity = Convert.ToDouble(opacity);

            var input = Path.Combine(_env.WebRootPath, @$"template\Admin\Docs\TestDocument.pdf");
            var output = Path.Combine(_env.WebRootPath, @$"template\Admin\Docs\TestTextWatermark.pdf");
            WatermarkHub.WatermarkProvider.AddTextWatermarkToFile(textWatermark, input, output);

            return View("TestTextWatermark", "Options");
        }
    }
}
