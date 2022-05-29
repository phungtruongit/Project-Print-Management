
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IUserApiClient {
        Task<ApiResultDTO<string>> AuthenticateAsync(LoginRequest request);
        Task<bool> CheckPermissionAsync(Guid id, int totalPage, string printerName, string token);
    }
}