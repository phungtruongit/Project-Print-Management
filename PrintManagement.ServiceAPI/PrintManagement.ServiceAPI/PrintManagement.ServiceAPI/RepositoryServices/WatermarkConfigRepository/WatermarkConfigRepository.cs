using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class WatermarkConfigRepository : IWatermarkConfigRepository {
        private readonly PrintManagementContext _context;

        public WatermarkConfigRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<WatermarkConfig>> GetImageWatermarkAsync() {
            try {
                var watermarkConfig = await _context.WatermarkConfigs.FirstOrDefaultAsync(x => x.Classify == 1);
                if (watermarkConfig == null) {
                    return new ApiResultDTO<WatermarkConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<WatermarkConfig>(true, "", watermarkConfig);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<WatermarkConfig>> GetTextWatermarkAsync() {
            try {
                var watermarkConfig = await _context.WatermarkConfigs.FirstOrDefaultAsync(x => x.Classify == 2);
                if (watermarkConfig == null) {
                    return new ApiResultDTO<WatermarkConfig>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<WatermarkConfig>(true, "", watermarkConfig);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateWatermarkAsync(WatermarkConfigDTO request) {
            try {
                var watermarkConfig = await _context.WatermarkConfigs.FirstOrDefaultAsync(x => x.Oid == request.Oid);
                if (watermarkConfig == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                watermarkConfig.Text = request.Text;
                watermarkConfig.FileImage = request.FileImage;
                watermarkConfig.FontSize = request.FontSize;
                watermarkConfig.FontFamily = request.FontFamily;
                watermarkConfig.VerticalAlignment = request.VerticalAlignment;
                watermarkConfig.HorizontalAlignment = request.HorizontalAlignment;
                watermarkConfig.Width = request.Width;
                watermarkConfig.Height = request.Height;
                watermarkConfig.X = request.X;
                watermarkConfig.Y = request.Y;
                watermarkConfig.IsBackground = request.IsBackground;
                watermarkConfig.Opacity = request.Opacity;
                watermarkConfig.RotateAngle = request.RotateAngle;

                _context.WatermarkConfigs.Update(watermarkConfig);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}