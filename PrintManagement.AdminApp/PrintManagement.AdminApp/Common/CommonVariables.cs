using PrintManagement.AdminApp.Models;
using PrintManagement.ApiIntegration;

namespace PrintManagement.AdminApp.Common {
    public class CommonVariables {
        public static List<VerticalAlignmentItem> g_lstVerticalAlignment = new List<VerticalAlignmentItem>() {
                new VerticalAlignmentItem { Id = 0, Name = "None"},
                new VerticalAlignmentItem { Id = 1, Name = "Top"},
                new VerticalAlignmentItem { Id = 2, Name = "Center" },
                new VerticalAlignmentItem { Id = 3, Name = "Bottom" }
        };

        public static List<HorizontalAlignmentItem> g_lstHorizontalAligment = new List<HorizontalAlignmentItem>()
        {
                new HorizontalAlignmentItem { Id = 0, Name = "None"},
                new HorizontalAlignmentItem { Id = 1, Name = "Left"},
                new HorizontalAlignmentItem { Id = 2, Name = "Center"},
                new HorizontalAlignmentItem { Id = 3, Name = "Right"}
        };

        public static readonly string g_connectionStringDB = "Data Source=.\\;Initial Catalog=PrintManagement;User ID=sa;Password=123456Aa;MultipleActiveResultSets=true";
        public static int g_UserConfig_DefaultBalance = 0;
        public static int g_UserConfig_DefaultRemainPages = 0;
        public static string g_BackupConfig_BackupLocation = String.Empty;
        public static int g_BackupConfig_BackupSchedule = 0;
        public static int g_BackupConfig_DeleteLogSchedule = 0;
        public static string g_FinancialConfig_CurrencyCode = String.Empty;
        public static int g_FinancialConfig_DefaultPrintCost = 0;
        public static WatermarkConfigDTO g_WatermarkConfig_ImageWatermark = new();
        public static WatermarkConfigDTO g_WatermarkConfig_TextWatermark = new();
    }
}
