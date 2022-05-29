
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IPrinterApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);

        Task<ApiResultDTO<List<PrinterDTO>>> GetAllPrinter();

        Task<ApiResultDTO<PagedResult<PrinterDTO>>> GetAllPrinter(string keyword, int pageSize, int pageIndex);

        Task<ApiResultDTO<bool>> InsertOrUpdatePrinter(List<PrinterDTO> Printer);

        Task<ApiResultDTO<bool>> UpdatePrinter(Guid id, PrinterDTO Printer);

        Task<ApiResultDTO<PrinterDTO>> GetPrinterById(Guid id);

        Task<ApiResultDTO<bool>> DeletePrinter(Guid id);
    }
}