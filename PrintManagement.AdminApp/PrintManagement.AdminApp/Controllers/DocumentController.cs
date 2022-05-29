using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

namespace PrintManagement.AdminApp.Controllers {
    public class DocumentController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IDocumentApiClient _documentApiClient;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public DocumentController(IDocumentApiClient documentApiClient,
            ILogger logger, BaseApiClient baseApiClient, IWebHostEnvironment webHostEnvironment) {
            _logger = logger;
            _documentApiClient = documentApiClient;
            _baseApiClient = baseApiClient;
            _env = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string keyword = "empty", int pageIndex = 1, int pageSize = 10) {
            if (string.IsNullOrEmpty(keyword))
                keyword = "empty";

            var data = await _documentApiClient.GetAllDocument(keyword, pageSize, pageIndex);

            if (!keyword.Equals("empty"))
                ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];

            if (TempData["error"] != null)
                ViewBag.ErrorMsg = TempData["error"];

            ViewData["Token"] = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);

            return View(data.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id) {
            var result = await _documentApiClient.GetDocumentById(id);
            return View(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> UploadDocument() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleUpload(DocumentUploadRequest document, IFormFile file) {
            string[] DocExtens = { ".PDF", ".DOC", ".DOCX", ".PPT", ".PPTX", ".XLS", ".XLSX", ".PNG", ".JPEG", ".TXT" };
            //if (!ModelState.IsValid)
            //    return View();

            var ext = Path.GetExtension(file.FileName);
            if (Array.IndexOf(DocExtens, ext.ToUpper()) == -1) {
                ModelState.AddModelError("", "Vui lòng chọn file đúng định dạng cho phép !!");
                return View("UploadDocument");
            }

            if (file.Length > int.MaxValue) {
                ModelState.AddModelError("", "File upload vượt quá giới hạn 2GB !!");
                return View("UploadDocument");
            }

            if (file.Length > 0) {
                var uniqueFileName = "";
                var uniqueFile = DocumentStatic.GetUniqueFileName(file.FileName);
                try {
                    var uploads = Path.Combine(_env.WebRootPath, @"ContentRepository\Documents");
                    var filePath = Path.Combine(uploads, file.FileName.Replace($"{ext}", $"_temp{ext}"));
                    uniqueFileName = file.FileName;
                    if (System.IO.File.Exists(filePath.Replace($"_temp{ext}", $"{ext}"))) {
                        ModelState.AddModelError("", "File trùng tên đã được lưu trữ, vui lòng đổi tên file !!");
                        return View("UploadDocument");
                        //filePath = Path.Combine(uploads, uniqueFile);
                        //uniqueFileName = uniqueFile;
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await file.CopyToAsync(stream);
                    }

                    // add Watermark
                    ImageWatermark imageWatermark = new ImageWatermark(@"H:\DEV\Web\Project Web\DATN Print Management\SourceCode\Git\PrintManagement\PrintManagement.AdminApp\PrintManagement.AdminApp\wwwroot\template\Admin\Images\GreenPrintHT_whiteBg.png") {
                        Opacity = 0.7,
                        X = 70,
                        Y = 350,
                        IsBackground = true
                    };
                    var fileInput = filePath;
                    var fileOutput = filePath.Replace($"_temp{ext}", $"{ext}");
                    WatermarkHub.WatermarkProvider.AddImageWatermarkToFile(imageWatermark, fileInput, fileOutput);

                    System.IO.File.Delete(filePath);

                }
                catch (Exception e) {
                    return Redirect("/User/Forbidden");
                }
            }

            document.Name = file.FileName;
            document.MimeType = ext.ToLower();
            document.SizeKb = file.Length.ToSize(DocumentStatic.SizeUnits.KB);
            document.IsUsageAllowed = true;
            document.CreatedBy = User.Claims.FirstOrDefault()?.Value;
            document.CreatedDate = DateTime.Now;
            document.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            document.ModifiedDate = DateTime.Now;
            document.IdUser = new Guid(User.Claims.First(x => x.Type.Equals("UserId")).Value);

            var result = await _documentApiClient.CreateDocument(document);
            if (result.IsSuccessed) {
                TempData["result"] = "Thêm mới tài liệu thành công";
                return RedirectToAction("Index", "Document");
            }

            ModelState.AddModelError("", result.Message);
            return View(document);
        }

        public async Task<IActionResult> DownloadDocument(Guid id) {
            var token = HttpContext.Session.GetString(SystemConstants.AppSetting.Token);
            var fileNameDocument = await CommonDataProvider.GetNameDocumentByIdAsync(id, token);
            var path = Path.Combine(_env.WebRootPath, @"ContentRepository\Documents", fileNameDocument);

            if (!System.IO.File.Exists(path)) {
                return Content("Không tìm thấy file");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open)) {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path) {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes() {
            return new Dictionary<string, string>
            {
                {".pdf", "application/pdf"},
                {".txt", "text/plain"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpeg", "image/jpeg"},
            };
        }

        public async Task<IActionResult> Delete(Guid id) {
            if (!ModelState.IsValid)
                return View();
            try {
                var result = await _documentApiClient.DeleteDocument(id);
                if (result.IsSuccessed) {
                    TempData["result"] = "Xóa tài liệu thành công";
                }
                ModelState.AddModelError("", result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                TempData["error"] = "Đối tượng đã phát sinh dữ liệu không thể xoá!";
                return RedirectToAction("Index");
            }
        }
    }
}
