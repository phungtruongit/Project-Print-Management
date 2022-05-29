using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class EmailConfigRepository : IEmailConfigRepository
    {
        private readonly PrintManagementContext _context;

        public EmailConfigRepository(PrintManagementContext context)
        {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var emailConfig = await _context.EmailConfigs.FindAsync(id);
                if (emailConfig == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.EmailConfigs.Remove(emailConfig);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<EmailConfig>> GetMain() {
            try {
                var systemInfo = await _context.EmailConfigs.FirstOrDefaultAsync();
                if (systemInfo == null) {
                    return new ApiResultDTO<EmailConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<EmailConfig>(true, "", systemInfo);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<EmailConfig>>> GetAllAsync()
        {
            var emailConfigs = await _context.EmailConfigs.ToListAsync();
            return emailConfigs.Envelope();
        }

        public async Task<ApiResultDTO<EmailConfig>> GetByIdAsync(Guid id)
        {
            try
            {
                var emailConfig = await _context.EmailConfigs.FindAsync(id);
                if (emailConfig == null)
                {
                    return new ApiResultDTO<EmailConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<EmailConfig>(true, "", emailConfig);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(EmailConfigDTO request)
        {
            try
            {
                var emailConfig = new EmailConfig()
                {
                    Oid = request.Oid,
                    Host = request.Host,
                    Port = request.Port,
                    UserName = request.UserName,
                    Password = request.Password,
                    SenderName = request.SenderName,
                };

                await _context.EmailConfigs.AddAsync(emailConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, EmailConfigDTO request)
        {
            try
            {
                var emailConfig = await _context.EmailConfigs.FindAsync(id);
                if (emailConfig == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                emailConfig.Host = request.Host;
                emailConfig.Port = request.Port;
                emailConfig.UserName = request.UserName;
                emailConfig.Password = request.Password;
                emailConfig.SenderName = request.SenderName;

                _context.EmailConfigs.Update(emailConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateMain(EmailConfigDTO request) {
            try {
                var emailConfig = await _context.EmailConfigs.FirstOrDefaultAsync();
                if (emailConfig == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                emailConfig.Host = request.Host;
                emailConfig.Port = request.Port;
                emailConfig.UserName = request.UserName;
                emailConfig.Password = request.Password;
                emailConfig.SenderName = request.SenderName;

                _context.EmailConfigs.Update(emailConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
