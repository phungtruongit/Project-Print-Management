
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration {
    public interface IDepartmentApiClient {
        Task<ApiResultDTO<string>> Authenticate(LoginRequest request);

        Task<ApiResultDTO<PagedResult<DepartmentDTO>>> GetAllDepartment(string keyword, int pageSize, int pageIndex);

        Task<ApiResultDTO<bool>> CreateDepartment(DepartmentDTO department);

        Task<ApiResultDTO<bool>> UpdateDepartment(Guid id, DepartmentDTO department);

        Task<ApiResultDTO<DepartmentDTO>> GetDepartmentById(Guid id);

        Task<ApiResultDTO<bool>> DeleteDepartment(Guid id);
    }
}