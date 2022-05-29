using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class SystemInfoRepository : ISystemInfoRepository {
        private readonly PrintManagementContext _context;

        public SystemInfoRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id) {
            try {
                var user = await _context.Users.FindAsync(id);
                if (user == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<SystemInfo>> GetSystemInfoMainAsync() {
            try {
                var systemInfo = await _context.SystemInfos.FirstOrDefaultAsync();
                if (systemInfo == null) {
                    return new ApiResultDTO<SystemInfo>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<SystemInfo>(true, "", systemInfo);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<SystemInfo>>> GetAllAsync() {
            var userGroups = await _context.SystemInfos.ToListAsync();
            return new ApiResultDTO<List<SystemInfo>>(true, "", userGroups);
        }

        public async Task<ApiResultDTO<SystemInfo>> GetByIdAsync(Guid id) {
            try {
                var userGroup = await _context.SystemInfos.FindAsync(id);
                if (userGroup == null) {
                    return new ApiResultDTO<SystemInfo>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<SystemInfo>(true, "", userGroup);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(SystemInfoDTO request) {
            try {
                var systemInfo = new SystemInfo() {
                    Oid = request.Oid,
                    Name = request.Name,
                    Version = request.Version,
                    Author = request.Author,
                    Status = request.Status,
                    Phone = request.Phone,
                    Email = request.Email,
                    TotalUser = request.TotalUser,
                    TotalMoneyProvided = request.TotalMoneyProvided,
                    TotalPrinter = request.TotalPrinter,
                    TotalFilePrinted = request.TotalFilePrinted,
                    TotalFileUploaded = request.TotalFileUploaded,
                    TotalFileDownload = request.TotalFileDownload,
                };

                await _context.SystemInfos.AddAsync(systemInfo);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, SystemInfoDTO request) {
            try {
                var systemInfo = await _context.SystemInfos.FindAsync(id);
                if (systemInfo == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                systemInfo.Name = request.Name;
                systemInfo.Version = request.Version;
                systemInfo.Author = request.Author;
                systemInfo.Status = request.Status;
                systemInfo.Phone = request.Phone;
                systemInfo.Email = request.Email;
                systemInfo.TotalUser = request.TotalUser;
                systemInfo.TotalMoneyProvided = request.TotalMoneyProvided;
                systemInfo.TotalPrinter = request.TotalPrinter;
                systemInfo.TotalFilePrinted = request.TotalFilePrinted;
                systemInfo.TotalFileUploaded = request.TotalFileUploaded;
                systemInfo.TotalFileDownload = request.TotalFileDownload;

                _context.SystemInfos.Update(systemInfo);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
