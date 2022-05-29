using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IEmailConfigRepository {
        Task<ApiResultDTO<EmailConfig>> GetMain();
        Task<ApiResultDTO<bool>> UpdateMain(EmailConfigDTO request);
        Task<ApiResultDTO<List<EmailConfig>>> GetAllAsync();
        Task<ApiResultDTO<EmailConfig>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(EmailConfigDTO request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, EmailConfigDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
