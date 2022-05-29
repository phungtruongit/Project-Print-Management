using PrintManagement.ApiIntegration;
using PrintManagement.EmailHub.Models;
using PrintManagement.MailHub.EmailProvider;
using System.Data.SqlClient;

namespace PrintManagement.AdminApp.Common {
    public class CommonBussiness {
        public async static Task GetCommonConfig(IUserConfigApiClient userConfigApiClient, IEmailConfigApiClient emailConfigApiClient, IBackupConfigApiClient backupConfigApiClient, IFinancialConfigApiClient financialConfigApiClient, IWatermarkConfigApiClient watermarkConfigApiClient) {
            var userConfig = (await userConfigApiClient.GetUserConfig()).ResultObj;
            var emailConfig = (await emailConfigApiClient.GetEmailConfig()).ResultObj;
            var backupConfig = (await backupConfigApiClient.GetBackupConfig()).ResultObj;
            var financialConfig = (await financialConfigApiClient.GetFinancialConfig()).ResultObj;
            var imageWatermarkConfig = (await watermarkConfigApiClient.GetImageWatermarkConfig()).ResultObj;
            var textWatermarkConfig = (await watermarkConfigApiClient.GetTextWatermarkConfig()).ResultObj;

            CommonVariables.g_UserConfig_DefaultBalance = Convert.ToInt32(userConfig.DefaultBalance);
            CommonVariables.g_UserConfig_DefaultRemainPages = Convert.ToInt32(userConfig.DefaultRemainPages);

            CommonVariables.g_BackupConfig_BackupLocation = backupConfig.BackupLocation;
            CommonVariables.g_BackupConfig_BackupSchedule = Convert.ToInt32(backupConfig.BackupSchedule);
            CommonVariables.g_BackupConfig_DeleteLogSchedule = Convert.ToInt32(backupConfig.DeleteLogSchedule);

            CommonVariables.g_FinancialConfig_CurrencyCode = financialConfig.CurrencyCode;
            CommonVariables.g_FinancialConfig_DefaultPrintCost = Convert.ToInt32(financialConfig.DefaultPrintCost);

            CommonVariables.g_WatermarkConfig_ImageWatermark = imageWatermarkConfig;
            CommonVariables.g_WatermarkConfig_TextWatermark = textWatermarkConfig;
        }

        public static string RandomString(int length) {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void TestMailSMTP(IEmailSender emailSender, string recipientAddress) {
            var message = new Message(new string[] { recipientAddress }, "GreenPrint HT Test Mail", null, null);
            var emailBody = new EmailBody() {
                MainContent = "Welcome to GreenPrint HT"
            };
            emailSender.SendEmail(message, emailBody);
        }

        public static void SendMailNewUser(IEmailSender emailSender, string recipientAddress, string recipientName, string username, string password) {
            var message = new Message(new string[] { recipientAddress }, "Chào mừng người dùng mới", null, null);
            var emailBody = new EmailBody() {
                RecipientName = recipientName,
                MainContent = $"Chào mừng người dùng mới của GreenPrint. Tài khoản của bạn được cấp bởi quản trị viên hệ thống. Username: {username}, Password: {password}. Đổi mật khẩu và bảo mật tài khoản của bạn. Liên hệ quản trị viên hệ thống khi gặp sự cố."
            };
            emailSender.SendEmail(message, emailBody);
        }

        public static void BackupDatabase() {
            var con = new SqlConnection(Common.CommonVariables.g_connectionStringDB);
            con.Open();
            var cmd = @$"BACKUP DATABASE [{con.Database}] TO DISK='{Common.CommonVariables.g_BackupConfig_BackupLocation}\PM_backup_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.bak'";
            var command = new SqlCommand(cmd, con);
            command.ExecuteNonQuery();
            con.Close();
        }

        public static void RestoreDatabase(string fileBackup) {
            var con = new SqlConnection(Common.CommonVariables.g_connectionStringDB);
            con.Open();
            var databaseName = con.Database;

            var cmd1 = $@"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            var command1 = new SqlCommand(cmd1, con);
            command1.ExecuteNonQuery();

            var cmd2 = $@"USE master RESTORE DATABASE [{databaseName}] FROM DISK='{Common.CommonVariables.g_BackupConfig_BackupLocation}\{fileBackup}' WITH REPLACE";
            var command2 = new SqlCommand(cmd2, con);
            command2.ExecuteNonQuery();

            var cmd3 = $@"ALTER DATABASE [{databaseName}] SET MULTI_USER";
            var command3 = new SqlCommand(cmd3, con);
            command3.ExecuteNonQuery();

            con.Close();
        }

        public static void ConvertDocumentToPDF(string fileInput, string fileOutput) {
            // TODO ...
        }
    }
}
