
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface ISystemInfoApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);

        Task<ApiResultDTO<List<SystemInfoDTO>>> GetAll();

        Task<ApiResultDTO<bool>> CreateSystemInfo(SystemInfoDTO SystemInfo);

        Task<ApiResultDTO<bool>> UpdateSystemInfo(Guid id, SystemInfoDTO SystemInfo);

        Task<ApiResultDTO<SystemInfoDTO>> GetSystemInfoById(Guid id);
        
        Task<ApiResultDTO<SystemInfoDTO>> GetSystemInfoMain();

        Task<ApiResultDTO<bool>> DeleteSystemInfo(Guid id);
    }
}