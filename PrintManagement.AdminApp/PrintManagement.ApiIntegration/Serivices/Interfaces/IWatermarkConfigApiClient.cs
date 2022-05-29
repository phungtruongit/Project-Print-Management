
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IWatermarkConfigApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);
        Task<ApiResultDTO<bool>> UpdateWatermarkConfig(WatermarkConfigDTO BackupConfig);
        Task<ApiResultDTO<WatermarkConfigDTO>> GetImageWatermarkConfig();
        Task<ApiResultDTO<WatermarkConfigDTO>> GetTextWatermarkConfig();
    }
}