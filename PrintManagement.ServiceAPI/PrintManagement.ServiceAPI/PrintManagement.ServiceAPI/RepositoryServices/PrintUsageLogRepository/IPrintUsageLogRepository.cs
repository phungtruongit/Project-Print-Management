using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IPrintUsageLogRepository {
        Task<ApiResultDTO<List<PrinterUsageLogDTO>>> GetAllAsync();
        Task<ApiResultDTO<PagedResult<PrinterUsageLog>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex, string userId = "");
        Task<ApiResultDTO<PrinterUsageLog>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(PrinterUsageLogDTO request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, PrinterUsageLogDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
