using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class DepartmentRepository : IDepartmentRepository {
        private readonly PrintManagementContext _context;

        public DepartmentRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id) {
            try {
                var department = await _context.Departments.FindAsync(id);
                if (department == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<Department>>> GetAllAsync() {
            var departments = await _context.Departments.OrderBy(x => x.CreatedDate).ToListAsync();
            return departments.Envelope();
        }

        public async Task<ApiResultDTO<PagedResult<Department>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex) {
            var query = _context.Departments.OrderBy(x => x.CreatedDate).AsQueryable();

            if (!keyword.Equals("empty")) {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<Department>() {
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data
            };
            return new ApiResultDTO<PagedResult<Department>>(true, "", pagedResult);
        }

        public async Task<ApiResultDTO<Department>> GetByIdAsync(Guid id) {
            try {
                var department = await _context.Departments.FindAsync(id);
                if (department == null) {
                    return new ApiResultDTO<Department>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<Department>(true, "", department);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(DepartmentDTO request) {
            try {
                var department = new Department() {
                    Oid = request.Oid,
                    Name = request.Name,
                    Note = request.Note,
                    CreatedBy = request.CreatedBy,
                    CreatedDate = request.CreatedDate,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedDate = request.ModifiedDate,
                };

                await _context.Departments.AddAsync(department);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, DepartmentDTO request) {
            try {
                var department = await _context.Departments.FindAsync(id);
                if (department == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                department.Name = request.Name;
                department.Note = request.Note;
                department.ModifiedBy = request.ModifiedBy;
                department.ModifiedDate = request.ModifiedDate;

                _context.Departments.Update(department);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
