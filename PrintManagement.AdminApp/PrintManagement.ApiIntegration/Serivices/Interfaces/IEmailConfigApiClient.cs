
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IEmailConfigApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);
        Task<ApiResultDTO<bool>> UpdateEmailConfig(EmailConfigDTO EmailConfig);
        Task<ApiResultDTO<EmailConfigDTO>> GetEmailConfig();
    }
}