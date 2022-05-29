using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public interface IUserGroupRepository
    {
        Task<ApiResultDTO<List<UserGroup>>> GetAllAsync();
        Task<ApiResultDTO<UserGroup>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(UserGroupDTO userGroup);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, UserGroupDTO userGroup);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
