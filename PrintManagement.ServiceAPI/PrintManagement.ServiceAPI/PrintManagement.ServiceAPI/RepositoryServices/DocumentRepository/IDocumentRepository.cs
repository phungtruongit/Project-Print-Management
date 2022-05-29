using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public interface IDocumentRepository {
        Task<ApiResultDTO<List<Document>>> GetAllAsync();
        Task<ApiResultDTO<PagedResult<Document>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex, string userId = "");
        Task<ApiResultDTO<Document>> GetByIdAsync(Guid id);
        Task<ApiResultDTO<bool>> InsertAsync(DocumentUploadRequest request);
        Task<ApiResultDTO<bool>> UpdateAsync(Guid id, DocumentDTO request);
        Task<ApiResultDTO<bool>> DeleteAsync(Guid id);
    }
}
