using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface ISystemInfoRepository {
        Task<ApiResultDTO<SystemInfo>> GetSystemInfoMainAsync();
        Task<ApiResultDTO<List<SystemInfo>>> GetAllAsync();
        Task<ApiResultDTO<SystemInfo>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(SystemInfoDTO SystemInfoDTO);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, SystemInfoDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
