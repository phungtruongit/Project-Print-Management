using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
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
    public class PrinterController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly IPrinterApiClient _PrinterApiClient;
        private readonly ILogger _logger;

        public PrinterController(IPrinterApiClient printerApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _PrinterApiClient = printerApiClient;
            _baseApiClient = baseApiClient;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string keyword = "empty", int pageIndex = 1, int pageSize = 10) {
            if (string.IsNullOrEmpty(keyword))
                keyword = "empty";

            var data = await _PrinterApiClient.GetAllPrinter(keyword, pageSize, pageIndex);

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
            var result = await _PrinterApiClient.GetPrinterById(id);
            return View(result.ResultObj);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create() {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PrinterDTO Printer) {
            if (!ModelState.IsValid)
                return View();

            Printer.CreatedBy = User.Claims.FirstOrDefault()?.Value;
            Printer.CreatedDate = DateTime.Now;
            Printer.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            Printer.ModifiedDate = DateTime.Now;

            //var result = await _PrinterApiClient.UpdatePrinter(Printer);
            //if (result.IsSuccessed) {
            //    TempData["result"] = "Thêm mới thiết bị thành công";
            //    return RedirectToAction("Index");
            //}

            ModelState.AddModelError("", "");
            return View(Printer);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id) {
            var result = await _PrinterApiClient.GetPrinterById(id);
            if (result.IsSuccessed) {
                return View(result.ResultObj);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(PrinterDTO request) {
            if (!ModelState.IsValid)
                return View();

            request.ModifiedBy = User.Claims.FirstOrDefault()?.Value;
            request.ModifiedDate = DateTime.Now;

            var result = await _PrinterApiClient.UpdatePrinter(request.Oid, request);
            if (result.IsSuccessed) {
                TempData["result"] = "Cập nhật thiết bị thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id) {
            if (!ModelState.IsValid)
                return View();

            var result = await _PrinterApiClient.DeletePrinter(id);
            if (result.IsSuccessed) {
                TempData["result"] = "Xóa thiết bị thành công";
            }
            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public async Task<IActionResult> UpdatePrinter() {
            var lstPrinter = new List<PrinterDTO>();
            var printerCollection = GetPrintersCollection();


            foreach (ManagementObject printer in printerCollection) {
                lstPrinter.Add(new PrinterDTO {
                    Oid = Guid.NewGuid(),
                    Name = Convert.ToString(printer.GetPropertyValue("Name"))?.Split('\\').LastOrDefault(),
                    DriverName = Convert.ToString(printer.GetPropertyValue("DriverName")),
                    ServerName = Convert.ToString(printer.GetPropertyValue("ServerName")),
                    PortName = Convert.ToString(printer.GetPropertyValue("PortName")),
                    Location = Convert.ToString(printer.GetPropertyValue("Location")),
                    IsNetwork = Convert.ToBoolean(printer.GetPropertyValue("Network")),
                    IsDefaultPrinter = Convert.ToBoolean(printer.GetPropertyValue("Default")),
                    Note = Convert.ToString(printer.GetPropertyValue("Description")),
                    CreatedBy = User.Claims.FirstOrDefault()?.Value,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = User.Claims.FirstOrDefault()?.Value,
                    ModifiedDate = DateTime.Now,
                });
            }

            var result = await _PrinterApiClient.InsertOrUpdatePrinter(lstPrinter);
            if (result.IsSuccessed) {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Error", "Home");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public static ManagementObjectCollection GetPrintersCollection() {
            ManagementScope objMS = new ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            ManagementObjectCollection printerCollection = objMOS.Get();

            return printerCollection;
        }
    }

    //class PrintSpoolerApi {
    //    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
    //    public static extern bool OpenPrinter(
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        string printerName,
    //        out IntPtr printerHandle,
    //        PrinterDefaults printerDefaults);

    //    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
    //    public static extern bool GetPrinter(
    //        IntPtr printerHandle,
    //        int level,
    //        IntPtr printerData,
    //        int bufferSize,
    //        ref int printerDataSize);

    //    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
    //    public static extern bool GetJob(
    //        IntPtr printerHandle,
    //        int jobId,
    //        int level,
    //        IntPtr pJob,
    //        int bufferSize,
    //        ref int printerDataSize);

    //    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
    //    public static extern bool ClosePrinter(
    //        IntPtr printerHandle);

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct PrinterDefaults {
    //        public IntPtr pDatatype;
    //        public IntPtr pDevMode;
    //        public int DesiredAccess;
    //    }

    //    public enum PrinterProperty {
    //        ServerName,
    //        PrinterName,
    //        ShareName,
    //        PortName,
    //        DriverName,
    //        Comment,
    //        Location,
    //        PrintProcessor,
    //        Datatype,
    //        Parameters,
    //        Attributes,
    //        Priority,
    //        DefaultPriority,
    //        StartTime,
    //        UntilTime,
    //        Status,
    //        Jobs,
    //        AveragePpm
    //    };

    //    public struct PrinterInfo2 {
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string ServerName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string PrinterName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string ShareName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string PortName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string DriverName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string Comment;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string Location;
    //        public IntPtr DevMode;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string SepFile;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string PrintProcessor;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string Datatype;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        public string Parameters;
    //        public IntPtr SecurityDescriptor;
    //        public uint Attributes;
    //        public uint Priority;
    //        public uint DefaultPriority;
    //        public uint StartTime;
    //        public uint UntilTime;
    //        public uint Status;
    //        public uint Jobs;
    //        public uint AveragePpm;
    //    }

    //    public static PrinterInfo2 GetPrinterProperty(string printerUncName) {
    //        var printerInfo2 = new PrinterInfo2();

    //        var pHandle = new IntPtr();
    //        var defaults = new PrinterDefaults();
    //        try {
    //            //Open a handle to the printer
    //            bool ok = OpenPrinter(printerUncName, out pHandle, defaults);

    //            if (!ok) {
    //                //OpenPrinter failed, get the last known error and thrown it
    //                throw new Win32Exception(Marshal.GetLastWin32Error());
    //            }

    //            //Here we determine the size of the data we to be returned
    //            //Passing in 0 for the size will force the function to return the size of the data requested
    //            int actualDataSize = 0;
    //            GetPrinter(pHandle, 2, IntPtr.Zero, 0, ref actualDataSize);

    //            int err = Marshal.GetLastWin32Error();

    //            if (err == 122) {
    //                if (actualDataSize > 0) {
    //                    //Allocate memory to the size of the data requested
    //                    IntPtr printerData = Marshal.AllocHGlobal(actualDataSize);
    //                    //Retrieve the actual information this time
    //                    GetPrinter(pHandle, 2, printerData, actualDataSize, ref actualDataSize);

    //                    //Marshal to our structure
    //                    printerInfo2 = (PrinterInfo2)Marshal.PtrToStructure(printerData, typeof(PrinterInfo2));
    //                    //We've made the conversion, now free up that memory
    //                    Marshal.FreeHGlobal(printerData);
    //                }
    //            }
    //            else {
    //                throw new Win32Exception(err);
    //            }

    //            return printerInfo2;
    //        }
    //        finally {
    //            //Always close the handle to the printer
    //            ClosePrinter(pHandle);
    //        }
    //    }
    //}
}