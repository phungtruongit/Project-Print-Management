using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly PrintManagementContext _context;

        public UserGroupRepository(PrintManagementContext context)
        {
            _context = context;
        }
        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var userGroup = await _context.UserGroups.FindAsync(id);
                if (userGroup == null)
                {
                    return new ApiResultDTO<bool>(false, "User Group không tồn tại");
                }
                _context.UserGroups.Remove(userGroup);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá User thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<UserGroup>>> GetAllAsync()
        {
            var userGroups = await _context.UserGroups.ToListAsync();
            return new ApiResultDTO<List<UserGroup>>(true, "", userGroups);
        }

        public async Task<ApiResultDTO<UserGroup>> GetByIdAsync(Guid id)
        {
            try
            {
                var userGroup = await _context.UserGroups.FindAsync(id);
                if (userGroup == null)
                {
                    return new ApiResultDTO<UserGroup>(false, "User group không tồn tại");
                }
                return new ApiResultDTO<UserGroup>(true, "", userGroup);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(UserGroupDTO request)
        {
            try
            {
                var userGroup = new UserGroup()
                {
                    Oid = request.Oid,
                    Name = request.Name,
                    Note = request.Note,
                    CreatedBy = request.CreatedBy,
                    CreatedDate = request.CreatedDate,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedDate = request.ModifiedDate,
                };

                await _context.AddAsync(userGroup);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, UserGroupDTO request)
        {
            try
            {
                var userGroup = await _context.UserGroups.FindAsync(id);
                if (userGroup == null)
                {
                    return new ApiResultDTO<bool>(false, "User group không tồn tại");
                }

                userGroup.Name = request.Name;
                userGroup.Note = request.Note;
                userGroup.CreatedBy = request.CreatedBy;
                userGroup.CreatedDate = request.CreatedDate;
                userGroup.ModifiedBy = request.ModifiedBy;
                userGroup.ModifiedDate = request.ModifiedDate;

                _context.UserGroups.Update(userGroup);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
