
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IBackupConfigApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);
        Task<ApiResultDTO<bool>> UpdateBackupConfig(BackupConfigDTO BackupConfig);
        Task<ApiResultDTO<BackupConfigDTO>> GetBackupConfig();
    }
}