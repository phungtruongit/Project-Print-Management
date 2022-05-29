using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IDepartmentRepository {
        Task<ApiResultDTO<List<Department>>> GetAllAsync();
        Task<ApiResultDTO<PagedResult<Department>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex);
        Task<ApiResultDTO<Department>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(DepartmentDTO request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, DepartmentDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
