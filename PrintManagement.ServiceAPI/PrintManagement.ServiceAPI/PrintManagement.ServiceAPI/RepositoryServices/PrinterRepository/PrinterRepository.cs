using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class PrinterRepository : IPrinterRepository {
        private readonly PrintManagementContext _context;

        public PrinterRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id) {
            try {
                var printer = await _context.Printers.FindAsync(id);
                if (printer == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.Printers.Remove(printer);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<PrinterDTO>>> GetAllAsync() {
            var printers = await _context.Printers.ToListAsync();
            var printersDTO = printers.Select(x => new PrinterDTO {
                Oid = x.Oid,
                Name = x.Name,
                DriverName = x.DriverName,
                ServerName = x.ServerName,
                PortName = x.PortName,
                IsNetwork = Convert.ToBoolean(x.IsNetwork),
                Location = x.Location,
                IsDefaultPrinter =Convert.ToBoolean(x.IsDefaultPrinter),
                DefaultCost = x.DefaultCost,
                TotalJobs = x.TotalJobs,
                TotalPages = x.TotalPages,
                Note = x.Note,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                IdDepartment = x.IdDepartment,
            }).ToList();
            return printersDTO.Envelope();
        }

        public async Task<ApiResultDTO<PagedResult<Printer>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex) {
            var query = _context.Printers.OrderBy(x => x.CreatedDate).AsQueryable();

            if (!keyword.Equals("empty")) {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<Printer>() {
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data
            };
            return new ApiResultDTO<PagedResult<Printer>>(true, "", pagedResult);
        }

        public async Task<ApiResultDTO<Printer>> GetByIdAsync(Guid id) {
            try {
                var printer = await _context.Printers.FindAsync(id);
                if (printer == null) {
                    return new ApiResultDTO<Printer>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<Printer>(true, "", printer);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(List<PrinterDTO> requests) {
            using var transaction = _context.Database.BeginTransaction();
            try {
                foreach (var request in requests) {
                    var financialConfig = _context.FinancialConfigs.FirstOrDefault();
                    var objPrinter = await _context.Printers.SingleOrDefaultAsync(x => x.Name.Equals(request.Name));
                    if (objPrinter == null) {
                        await _context.Printers.AddAsync(new Printer() {
                            Oid = request.Oid,
                            Name = request.Name,
                            DriverName = request.DriverName,
                            ServerName = request.ServerName,
                            PortName = request.PortName,
                            IsNetwork = request.IsNetwork,
                            Location = request.Location,
                            IsDefaultPrinter = request.IsDefaultPrinter,
                            DefaultCost =  financialConfig != null ? financialConfig.DefaultPrintCost : 100,
                            TotalJobs = 0,
                            TotalPages = 0,
                            Note = request.Note,
                            CreatedBy = request.CreatedBy,
                            CreatedDate = request.CreatedDate,
                            ModifiedBy = request.ModifiedBy,
                            ModifiedDate = request.ModifiedDate,
                            IdDepartment = string.IsNullOrEmpty(request.Location) ? null : GetIdDepartment(request.Location),
                        });
                    }
                    else {
                        objPrinter.Name = request.Name;
                        objPrinter.DriverName = request.DriverName;
                        objPrinter.ServerName = request.ServerName;
                        objPrinter.PortName = request.PortName;
                        objPrinter.IsNetwork = request.IsNetwork;
                        objPrinter.Location = request.Location;
                        objPrinter.IsDefaultPrinter = request.IsDefaultPrinter;
                        objPrinter.DefaultCost =  financialConfig != null ? financialConfig.DefaultPrintCost : 100;
                        objPrinter.Note = request.Note;
                        objPrinter.ModifiedBy = request.ModifiedBy;
                        objPrinter.ModifiedDate = request.ModifiedDate;
                        objPrinter.IdDepartment = string.IsNullOrEmpty(request.Location) ? null : GetIdDepartment(request.Location);

                        _context.Printers.Update(objPrinter);
                    }
                }

                await _context.SaveChangesAsync();
                transaction.Commit();
                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
        private static Guid? GetIdDepartment(string location) => location switch {
            "Phong Dao tao" => new Guid("0a13a877-72c2-41a2-84c7-2560fbd030ed"),
            "Phong Hanh chinh" => new Guid("09e57a44-cc45-47d7-891c-dd4266bc75e5"),
            "Phong Ke toan" => new Guid("7a395b51-4b9c-424b-8c12-2b669c4d09b0"),
            "Phong Trien khai" => new Guid("a762f8e0-1ff3-412b-beb9-569c065c62fc"),
            "Phong Nghien cuu" => new Guid("04945341-6db8-496e-8978-8fd2e82fdd85"),
            "Phong Cong nghe" => new Guid("56ff8626-152c-4ae0-9356-95352df97fcc"),
            "Phong Giam doc" => new Guid("3eed5fb2-fd3f-41f0-a8c5-2839a8e82a73"),
            _ => null,
        };

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, PrinterDTO request) {
            try {
                var printer = await _context.Printers.FindAsync(id);
                if (printer == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                printer.Name = request.Name;
                printer.DriverName = request.DriverName;
                printer.ServerName = request.ServerName;
                printer.PortName = request.PortName;
                printer.IsNetwork = request.IsNetwork;
                printer.Location = request.Location;
                printer.IsDefaultPrinter = request.IsDefaultPrinter;
                printer.TotalJobs = request.TotalJobs;
                printer.TotalPages = request.TotalPages;
                printer.LastUsageDate = request.LastUsageDate;
                printer.Note = request.Note;
                printer.CreatedBy = request.CreatedBy;
                printer.CreatedDate = request.CreatedDate;
                printer.ModifiedBy = request.ModifiedBy;
                printer.ModifiedDate = request.ModifiedDate;
                printer.IdDepartment = request.IdDepartment;

                _context.Printers.Update(printer);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
