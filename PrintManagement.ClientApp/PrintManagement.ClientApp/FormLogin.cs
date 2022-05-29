using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Toolkit.Uwp.Notifications;
using PrinterQueueWatch;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintManagement.ClientApp {
    public partial class FormLogin : Form {
        #region Variable & Constructors
        private readonly IUserApiClient _userApiClient;
        private readonly AppSettings _appSettings = new AppSettings {
            BaseAddress = "https://localhost:7201",
            Secretkey = "914aeb3ecc4f459d8b07825cf1a3cfb2"
        };
        private readonly ILoggerManager _logger;
        private readonly IPrinterUsageLogApiClient _printerUserLogApiClient;
        private ClaimsPrincipal _claimsPrincipal;
        private string _token;
        private Guid _idUser;
        private string _nameUser;
        private string _userLoginWindow;
        private int _totalPages = 0;


        public FormLogin(ILoggerManager logger) {
            InitializeComponent();
            _userApiClient = new UserApiClient(_appSettings);
            _printerUserLogApiClient = new PrinterUsageLogApiClient(_appSettings);
            _logger = logger;
        }

        public FormLogin() {
            InitializeComponent();
        }
        #endregion

        #region EventHandler
        private void FormLogin_Load(object sender, EventArgs e) {

        }

        private void linkLabelHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var url = "https://localhost:7016/Login/Index";
            System.Diagnostics.Process.Start(url);
        }

        private async void btnLogin_ClickAsync(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text)) {
                MessageBox.Show("Yêu cầu nhập đủ Username và Password", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var loginRequest = new LoginRequest() {
                UserName = txtUsername.Text,
                Password = txtPassword.Text,
            };

            var resLogin = await LoginAccount(loginRequest);

            if (!resLogin.IsSuccessed) // Login Fail !!!
            {
                MessageBox.Show(resLogin.Message, "Đăng Nhập Thất Bại!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError("Login Failed!");
                return;
            }
            else // Login Success !!!
            {
                var pathIcon = Directory.GetCurrentDirectory().Replace(@"bin\Debug", @"assets\icologo.ico");
                notifyIcon1.BalloonTipText = "GreenPrint HT Client đang chạy ...";
                notifyIcon1.BalloonTipTitle = "GreenPrint HT Client";
                notifyIcon1.Icon = new System.Drawing.Icon(pathIcon);
                notifyIcon1.Text = "GreenPrint HT Client";
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(10);
                notifyIcon1.ContextMenuStrip = contextMenuStrip1;
                this.Hide();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            notifyIcon1.Visible = false;
            this.Show();
        }
        #endregion

        #region Bussiness
        private async Task<ApiResultDTO<string>> LoginAccount(LoginRequest request) {
            try {
                var result = await _userApiClient.AuthenticateAsync(request);
                if (result.ResultObj == null) {
                    return result;
                }

                _token = result.ResultObj;
                _claimsPrincipal = this.ValidateToken(result.ResultObj);
                _idUser = Guid.Parse(_claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("UserId")).Value);
                _nameUser = _claimsPrincipal.Claims.FirstOrDefault().Value;
                _userLoginWindow = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').LastOrDefault();

                StartWatching();

                _logger.LogInfo($"Track Printer Started at: {DateTime.Now}");
                return result;
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - LoginAccount");
                throw;
            }
        }

        private void StartWatching() {
            var printer = new PrinterDTO();
            var listPrinter = GetListPrinter();

            foreach (var item in listPrinter) {
                var mPr = new PrinterMonitorComponent();
                //mPr.MonitorPrinterChangeEvent = false;
                mPr.JobAdded += AddedJob;
                try {
                    mPr.AddPrinter(item.Name);
                }
                catch (Exception ex) {
                    throw;
                }
            }
        }

        public async void AddedJob(object sender, PrintJobEventArgs e) {
            try {
                if (!e.PrintJob.UserName.Equals(_userLoginWindow, StringComparison.CurrentCultureIgnoreCase))
                    return;
                var printUsageLog = new PrinterUsageLogDTO();
                var printerName = e.PrintJob.PrinterName.Split('\\').LastOrDefault();

                while (e.PrintJob.StatusDescription == "") {
                    Thread.Sleep(10);
                }

                var isPermission = await _userApiClient.CheckPermissionAsync(_idUser, e.PrintJob.TotalPages, printerName, _token);

                if (!isPermission) {
                    if (e.PrintJob.MachineName.Contains(Environment.MachineName)) {
                        e.PrintJob.Cancel();
                        //e.PrintJob.Delete();
                        await InsertPrintUsageLogCancelled(e.PrintJob, printUsageLog);

                        new ToastContentBuilder()
                        .AddText("LỆNH IN ĐÃ BỊ HUỶ")
                        .AddText("Kiểm tra số dư hoặc quyền truy cập thiết bị!")
                        .Show(toast => {
                            toast.ExpirationTime = DateTime.Now.AddMinutes(15);
                        });

                        _logger.LogInfo("Job cancelled - AddedJob()");

                    }
                    //MessageBox.Show("Cancelled!");
                    return;
                }

                while (!e.PrintJob.Printed) {
                    Thread.Sleep(1);
                }

                //MessageBox.Show($"{e.PrintJob.MachineName} jobSize: {e.PrintJob.JobSize} - totalPages: {e.PrintJob.TotalPages}");
                _totalPages = _totalPages < e.PrintJob.TotalPages ? e.PrintJob.TotalPages : _totalPages;

                printUsageLog.UsageDate = e.PrintJob.Submitted;
                printUsageLog.TotalPages = _totalPages;
                printUsageLog.Copies = e.PrintJob.Copies;
                printUsageLog.JobStatus = e.PrintJob.StatusDescription;

                if (e.PrintJob.MachineName.Contains(Environment.MachineName)) {
                    printUsageLog.Oid = Guid.NewGuid();
                    printUsageLog.UsageBy = _nameUser;
                    printUsageLog.MachineName = Environment.MachineName;
                    printUsageLog.UserName = e.PrintJob.UserName;
                    printUsageLog.PrinterName = printerName;
                    printUsageLog.DriverName = e.PrintJob.DriverName;
                    printUsageLog.IsColor = e.PrintJob.Color;
                    printUsageLog.DocumentName = e.PrintJob.Document;
                    printUsageLog.PaperKind = e.PrintJob.PaperKind.ToString();
                    printUsageLog.PaperLength = e.PrintJob.PaperLength;
                    printUsageLog.PaperWidth = e.PrintJob.PaperWidth;
                    printUsageLog.PrintProcessorName = e.PrintJob.PrintProcessorName;

                    printUsageLog.IsLandscape = e.PrintJob.Landscape;
                    printUsageLog.IsDuplex = e.PrintJob.Duplex;
                    printUsageLog.IsPrinted = e.PrintJob.Printed;
                    printUsageLog.IsCancelled = false;

                    printUsageLog.IdUser = _idUser;
                    printUsageLog.JobSize = e.PrintJob.JobSize;
                    printUsageLog.JobId = e.PrintJob.JobId;
                    await _printerUserLogApiClient.CreatePrinterUsageLog(printUsageLog, _token);

                    //MessageBox.Show($"jobsize: {printUsageLog.JobSize} - totalPages: {_totalPages}");
                    //MessageBox.Show("Done");
                    _totalPages = 0;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, " - AddedJob");
            }
        }

        public async Task InsertPrintUsageLogCancelled(PrintJob printJob, PrinterUsageLogDTO printUsageLog) {
            try {
                printUsageLog.Oid = Guid.NewGuid();
                printUsageLog.UsageBy = _nameUser;
                printUsageLog.MachineName = Environment.MachineName;
                printUsageLog.UserName = printJob.UserName;
                printUsageLog.PrinterName = printJob.PrinterName;
                printUsageLog.DriverName = printJob.DriverName;
                printUsageLog.IsColor = printJob.Color;
                printUsageLog.DocumentName = printJob.Document;
                printUsageLog.PaperKind = printJob.PaperKind.ToString();
                printUsageLog.PaperLength = printJob.PaperLength;
                printUsageLog.PaperWidth = printJob.PaperWidth;
                printUsageLog.PrintProcessorName = printJob.PrintProcessorName;

                printUsageLog.IsLandscape = printJob.Landscape;
                printUsageLog.IsDuplex = printJob.Duplex;
                printUsageLog.IsPrinted = printJob.Printed;
                printUsageLog.IsCancelled = true;

                printUsageLog.IdUser = _idUser;
                printUsageLog.JobSize = printJob.JobSize;
                printUsageLog.JobId = printJob.JobId;

                printUsageLog.UsageDate = printJob.Submitted;
                printUsageLog.TotalPages = _totalPages;
                printUsageLog.Copies = printJob.Copies;
                printUsageLog.JobStatus = "Deleted";

                await _printerUserLogApiClient.CreatePrinterUsageLog(printUsageLog, _token);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public List<PrinterDTO> GetListPrinter() {
            var listPrinter = new List<PrinterDTO>();
            ManagementScope objMS = new ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            ManagementObjectCollection objMOC = objMOS.Get();

            foreach (ManagementObject Printers in objMOC) {
                var print = new PrinterDTO();
                print.IsDefaultPrinter = System.Convert.ToBoolean(Printers.GetPropertyValue("Default"));
                print.Name = Printers.GetPropertyValue("Name").ToString();

                listPrinter.Add(print);
            }

            return listPrinter;
        }
        private ClaimsPrincipal ValidateToken(string jwtToken) {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidateAudience = false;
            validationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secretkey));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
        #endregion
    }
}