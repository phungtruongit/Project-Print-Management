using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IPrinterRepository{
        Task<ApiResultDTO<List<PrinterDTO>>> GetAllAsync();
        Task<ApiResultDTO<PagedResult<Printer>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex);
        Task<ApiResultDTO<Printer>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(List<PrinterDTO> request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, PrinterDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
