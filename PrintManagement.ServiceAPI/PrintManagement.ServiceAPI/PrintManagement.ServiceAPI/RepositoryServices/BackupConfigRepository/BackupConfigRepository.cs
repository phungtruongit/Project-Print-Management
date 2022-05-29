using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class BackupConfigRepository : IBackupConfigRepository
    {
        private readonly PrintManagementContext _context;

        public BackupConfigRepository(PrintManagementContext context)
        {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var backupConfig = await _context.BackupConfigs.FindAsync(id);
                if (backupConfig == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.BackupConfigs.Remove(backupConfig);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<BackupConfig>>> GetAllAsync()
        {
            var backupConfigs = await _context.BackupConfigs.ToListAsync();
            return backupConfigs.Envelope();
        }

        public async Task<ApiResultDTO<BackupConfig>> GetByIdAsync(Guid id)
        {
            try
            {
                var backupConfig = await _context.BackupConfigs.FindAsync(id);
                if (backupConfig == null)
                {
                    return new ApiResultDTO<BackupConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<BackupConfig>(true, "", backupConfig);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<BackupConfig>> GetMain() {
            try {
                var backupConfig = await _context.BackupConfigs.FirstOrDefaultAsync();
                if (backupConfig == null) {
                    return new ApiResultDTO<BackupConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<BackupConfig>(true, "", backupConfig);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(BackupConfigDTO request)
        {
            try
            {
                var backupConfig = new BackupConfig()
                {
                    Oid = request.Oid,
                    BackupLocation = request.BackupLocation,
                    BackupSchedule = request.BackupSchedule,
                    DeleteLogSchedule = request.DeleteLogSchedule,
                };

                await _context.BackupConfigs.AddAsync(backupConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, BackupConfigDTO request)
        {
            try
            {
                var backupConfig = await _context.BackupConfigs.FindAsync(id);
                if (backupConfig == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                backupConfig.BackupLocation = request.BackupLocation;
                backupConfig.BackupSchedule = request.BackupSchedule;
                backupConfig.DeleteLogSchedule = request.DeleteLogSchedule;

                _context.BackupConfigs.Update(backupConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateMain(BackupConfigDTO request) {
            try {
                var backupConfig = await _context.BackupConfigs.FirstOrDefaultAsync();
                if (backupConfig == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                backupConfig.BackupLocation = request.BackupLocation;
                backupConfig.BackupSchedule = request.BackupSchedule;
                backupConfig.DeleteLogSchedule = request.DeleteLogSchedule;

                _context.BackupConfigs.Update(backupConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
