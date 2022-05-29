using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class FinancialConfigRepository : IFinancialConfigRepository {
        private readonly PrintManagementContext _context;

        public FinancialConfigRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<FinancialConfig>> GetMainAsync() {
            try {
                var financialConfig = await _context.FinancialConfigs.FirstOrDefaultAsync();
                if (financialConfig == null) {
                    return new ApiResultDTO<FinancialConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<FinancialConfig>(true, "", financialConfig);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateMainAsync(FinancialConfigDTO request) {
            try {
                var financialConfig = await _context.FinancialConfigs.FirstOrDefaultAsync();
                if (financialConfig == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                financialConfig.CurrencyCode = request.CurrencyCode;
                financialConfig.DefaultPrintCost = request.DefaultPrintCost;

                _context.FinancialConfigs.Update(financialConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}