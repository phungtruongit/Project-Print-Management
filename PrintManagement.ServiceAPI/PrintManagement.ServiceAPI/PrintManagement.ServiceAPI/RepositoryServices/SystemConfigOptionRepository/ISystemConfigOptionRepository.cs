using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface ISystemConfigOptionRepository {
        Task<ApiResultDTO<List<SystemConfigOption>>> GetAllAsync();
        Task<ApiResultDTO<SystemConfigOption>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(SystemConfigOptionDTO request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, SystemConfigOptionDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);

    }
}
