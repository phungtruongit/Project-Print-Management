using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IWatermarkConfigRepository {
        Task<ApiResultDTO<WatermarkConfig>> GetImageWatermarkAsync();
        Task<ApiResultDTO<WatermarkConfig>> GetTextWatermarkAsync();
        Task<ApiResultDTO<bool>> UpdateWatermarkAsync(WatermarkConfigDTO request);
    }
}
