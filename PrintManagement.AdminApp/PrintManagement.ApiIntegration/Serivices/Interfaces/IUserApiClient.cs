
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IUserApiClient {
        Task<ApiResultDTO<string>> AuthenticateAsync(LoginRequest request);

        Task<ApiResultDTO<List<UserDTO>>> GetAllUserAsync();

        Task<ApiResultDTO<PagedResult<UserDTO>>> GetUsersPagingsAsync(string keyword, int pageSize, int pageIndex);

        Task<ApiResultDTO<bool>> RegisterUserAsync(RegisterRequest registerRequest);

        Task<ApiResultDTO<bool>> UpdateUserAsync(Guid id, UserDTO request);

        Task<ApiResultDTO<UserDTO>> GetByIdAsync(Guid id);

        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);

        Task<ApiResultDTO<bool>> ResetUserAsync(ResetUserRequest request);
        Task<ApiResultDTO<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResultDTO<Guid>> GetUserRoleAsync(Guid idUser);
        Task<ApiResultDTO<List<RoleDTO>>> GetRolesAsync();
        Task<ApiResultDTO<bool>> RoleAssignAsync(RoleAssignRequest request);

        //Task<ApiResultDTO<string>> ForgetPasswordAsync(ForgetPasswordRequest request);
    }
}