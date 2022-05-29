using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IUserConfigRepository {
        Task<ApiResultDTO<UserConfig>> GetMainAsync();
        Task<ApiResultDTO<bool>> UpdateMainAsync(UserConfigDTO request);
    }
}
