
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IUserConfigApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);
        Task<ApiResultDTO<bool>> UpdateUserConfig(UserConfigDTO BackupConfig);
        Task<ApiResultDTO<UserConfigDTO>> GetUserConfig();
    }
}