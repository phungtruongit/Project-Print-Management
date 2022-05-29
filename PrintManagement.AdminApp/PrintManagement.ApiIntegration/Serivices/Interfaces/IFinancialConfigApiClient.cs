
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IFinancialConfigApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);
        Task<ApiResultDTO<bool>> UpdateFinancialConfig(FinancialConfigDTO BackupConfig);
        Task<ApiResultDTO<FinancialConfigDTO>> GetFinancialConfig();
    }
}