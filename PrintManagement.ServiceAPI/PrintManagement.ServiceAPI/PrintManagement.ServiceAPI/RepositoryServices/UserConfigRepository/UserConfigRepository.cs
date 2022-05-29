using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class UserConfigRepository : IUserConfigRepository
    {
        private readonly PrintManagementContext _context;

        public UserConfigRepository(PrintManagementContext context)
        {
            _context = context;
        }

        public async Task<ApiResultDTO<UserConfig>> GetMainAsync()
        {
            try {
                var userConfig = await _context.UserConfigs.FirstOrDefaultAsync();
                if (userConfig == null) {
                    return new ApiResultDTO<UserConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<UserConfig>(true, "", userConfig);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateMainAsync(UserConfigDTO request)
        {
            try {
                var userConfig = await _context.UserConfigs.FirstOrDefaultAsync();
                if (userConfig == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                var defaulCostPrinter = await _context.FinancialConfigs.FirstOrDefaultAsync();

                userConfig.DefaultBalance = request.DefaultBalance;
                userConfig.DefaultRemainPages = request.DefaultBalance / defaulCostPrinter?.DefaultPrintCost;

                _context.UserConfigs.Update(userConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}