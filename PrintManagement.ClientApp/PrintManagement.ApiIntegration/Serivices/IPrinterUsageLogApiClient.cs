
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IPrinterUsageLogApiClient {
        //Task<ApiResultDTO<string>> Authenticate(LoginRequest request);

        //Task<ApiResultDTO<List<PrinterUsageLogDTO>>> GetAllPrinterUsageLog();

        //Task<ApiResultDTO<PagedResult<PrinterUsageLogDTO>>> GetAllPrinterUsageLog (string keyword, int pageSize, int pageIndex);

        Task<ApiResultDTO<bool>> CreatePrinterUsageLog(PrinterUsageLogDTO PrinterUsageLog, string token);

        //Task<ApiResultDTO<bool>> UpdatePrinterUsageLog(Guid id, PrinterUsageLogDTO PrinterUsageLog);

        //Task<ApiResultDTO<PrinterUsageLogDTO>> GetPrinterUsageLogById(Guid id);

        //Task<ApiResultDTO<bool>> DeletePrinterUsageLog(Guid id);
    }
}