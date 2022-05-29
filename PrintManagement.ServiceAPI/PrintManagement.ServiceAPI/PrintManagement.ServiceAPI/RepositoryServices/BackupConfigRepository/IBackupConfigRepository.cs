using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IBackupConfigRepository {
        Task<ApiResultDTO<BackupConfig>> GetMain();
        Task<ApiResultDTO<bool>> UpdateMain(BackupConfigDTO request);
        Task<ApiResultDTO<List<BackupConfig>>> GetAllAsync();
        Task<ApiResultDTO<BackupConfig>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(BackupConfigDTO request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, BackupConfigDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
