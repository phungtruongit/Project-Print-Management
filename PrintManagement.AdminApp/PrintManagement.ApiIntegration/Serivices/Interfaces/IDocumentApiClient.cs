
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IDocumentApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);

        Task<ApiResultDTO<List<DocumentDTO>>> GetAllDocument();
        Task<ApiResultDTO<PagedResult<DocumentDTO>>> GetAllDocument(string keyword, int pageSize, int pageIndex);

        Task<ApiResultDTO<bool>> CreateDocument(DocumentUploadRequest Document);

        Task<ApiResultDTO<bool>> UpdateDocument(Guid id, DocumentDTO Document);

        Task<ApiResultDTO<DocumentDTO>> GetDocumentById(Guid id);

        Task<ApiResultDTO<bool>> DeleteDocument(Guid id);
    }
}