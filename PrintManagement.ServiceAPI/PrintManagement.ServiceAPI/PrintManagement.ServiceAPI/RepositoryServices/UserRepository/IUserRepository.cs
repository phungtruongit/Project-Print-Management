using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IUserRepository {
        Task<ApiResultDTO<string>> LoginAsync(LoginRequest login);
        Task<string> AuthenticateAsync(User user);
        Task<ApiResultDTO<bool>> RegisterAsync(RegisterRequest request);
        Task<ApiResultDTO<List<User>>> GetAllAsync();
        Task<ApiResultDTO<PagedResult<User>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex);
        Task<ApiResultDTO<User>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, UserDTO DTO);
        Task<ApiResultDTO<bool>> ResetUserAsync(ResetUserRequest request);
        Task<ApiResultDTO<bool>> ForgetPasswordAsync(ForgetPasswordRequest request);
        Task<ApiResultDTO<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<bool> CheckPrintPermissionAsync(Guid idUser, int totalPage, string printerFullName);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);

        // Roles handle for user
        Task<ApiResultDTO<Guid>> GetUserRoleAsync (Guid idUser);
        Task<ApiResultDTO<List<Role>>> GetRolesAsync();
        Task<ApiResultDTO<bool>> RoleAssignAsync(RoleAssignRequest roleAssignRequest);
    }
}
