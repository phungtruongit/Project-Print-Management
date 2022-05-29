using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class SystemConfigOptionRepository : ISystemConfigOptionRepository
    {
        private readonly PrintManagementContext _context;

        public SystemConfigOptionRepository(PrintManagementContext context)
        {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var systemConfigOption = await _context.SystemConfigOptions.FindAsync(id);
                if (systemConfigOption == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.SystemConfigOptions.Remove(systemConfigOption);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<SystemConfigOption>>> GetAllAsync()
        {
            var systemConfigOptions = await _context.SystemConfigOptions.ToListAsync();
            return new ApiResultDTO<List<SystemConfigOption>>(true, "", systemConfigOptions);
        }

        public async Task<ApiResultDTO<SystemConfigOption>> GetByIdAsync(Guid id)
        {
            try
            {
                var systemConfigOption = await _context.SystemConfigOptions.FindAsync(id);
                if (systemConfigOption == null)
                {
                    return new ApiResultDTO<SystemConfigOption>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<SystemConfigOption>(true, "", systemConfigOption);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(SystemConfigOptionDTO request)
        {
            try
            {
                var systemConfigOption = new SystemConfigOption()
                {
                    Oid = request.Oid,
                    IdBackupConfig = request.IdBackupConfig,
                    IdEmailConfig = request.IdEmailConfig,
                };

                await _context.SystemConfigOptions.AddAsync(systemConfigOption);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, SystemConfigOptionDTO request)
        {
            try
            {
                var systemConfigOption = await _context.SystemConfigOptions.FindAsync(id);
                if (systemConfigOption == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                systemConfigOption.IdBackupConfig = request.IdBackupConfig;
                systemConfigOption.IdEmailConfig = request.IdEmailConfig;

                _context.SystemConfigOptions.Update(systemConfigOption);
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
