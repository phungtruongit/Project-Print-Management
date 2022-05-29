using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IFinancialConfigRepository {
        Task<ApiResultDTO<FinancialConfig>> GetMainAsync();
        Task<ApiResultDTO<bool>> UpdateMainAsync(FinancialConfigDTO request);
    }
}
