using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly PrintManagementContext _context;

        public DocumentRepository(PrintManagementContext context)
        {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<Document>>> GetAllAsync()
        {
            var documents = await _context.Documents.ToListAsync();
            return documents.Envelope();
        }

        public async Task<ApiResultDTO<PagedResult<Document>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex, string userId = "")
        {
            var query = _context.Documents.OrderBy(x => x.CreatedDate).AsQueryable();

            // filter user
            if (userId != "")
                query = query.Where(x => x.IdUser.Equals(new Guid(userId)));

            if (!keyword.Equals("empty"))
            {
                query = query.Where(x => x.Name.Contains(keyword)
                    || x.MimeType.Contains(keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<Document>()
            {
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data
            };
            return new ApiResultDTO<PagedResult<Document>>(true, "", pagedResult);
        }

        public async Task<ApiResultDTO<Document>> GetByIdAsync(Guid id)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return new ApiResultDTO<Document>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<Document>(true, "", document);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(DocumentUploadRequest request)
        {
            try
            {
                var document = new Document()
                {
                    Oid = Guid.NewGuid(),
                    Name = request.Name,
                    TotalPages = request.TotalPages,
                    SizeKb = request.SizeKb,
                    PaperHeightMilimet = request.PaperHeightMilimet,
                    PaperWidthMilimet = request.PaperWidthMilimet,
                    MimeType = request.MimeType,
                    IsUsageAllowed = request.IsUsageAllowed,
                    Note = request.Note,
                    CreatedBy = request.CreatedBy,
                    CreatedDate = request.CreatedDate,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedDate = request.ModifiedDate,
                    IdUser = request.IdUser,
                };

                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, DocumentDTO request)
        {
            try
            {
                var document = await _context.Documents.FindAsync(id);
                if (document == null)
                {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                document.Name = request.Name;
                document.TotalPages = request.TotalPages;
                document.SizeKb = request.SizeKb;
                document.PaperHeightMilimet = request.PaperHeightMilimet;
                document.PaperWidthMilimet = request.PaperWidthMilimet;
                document.MimeType = request.MimeType;
                document.IsUsageAllowed = request.IsUsageAllowed;
                document.Note = request.Note;
                document.CreatedBy = request.CreatedBy;
                document.CreatedDate = request.CreatedDate;
                document.ModifiedBy = request.ModifiedBy;
                document.ModifiedDate = request.ModifiedDate;
                document.IdUser = request.IdUser;

                _context.Documents.Update(document);
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
