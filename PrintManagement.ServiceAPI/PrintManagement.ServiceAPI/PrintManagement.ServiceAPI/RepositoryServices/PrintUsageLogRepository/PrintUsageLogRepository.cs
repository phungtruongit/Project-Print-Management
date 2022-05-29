using Microsoft.EntityFrameworkCore;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class PrintUsageLogRepository : IPrintUsageLogRepository {
        private readonly PrintManagementContext _context;

        public PrintUsageLogRepository(PrintManagementContext context) {
            _context = context;
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id) {
            try {
                var printerUsageLog = await _context.PrinterUsageLogs.FindAsync(id);
                if (printerUsageLog == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }
                _context.PrinterUsageLogs.Remove(printerUsageLog);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<PrinterUsageLogDTO>>> GetAllAsync() {
            var printerUsageLogs = await _context.PrinterUsageLogs.OrderByDescending(x => x.UsageDate).ToListAsync();
            var printerUsageLogsDTO = printerUsageLogs.Select(x => new PrinterUsageLogDTO {
                UsageDate = x.UsageDate,
                UsageBy = x.UsageBy,
                UsageCost = x.UsageCost,
                MachineName = x.MachineName,
                JobId = x.JobId,
                UserName = x.UserName,
                JobStatus = x.JobStatus,
                Copies = x.Copies,
                TotalPages = x.TotalPages,
                PrinterName = x.PrinterName,
                DriverName = x.DriverName,
                IsColor = Convert.ToBoolean(x.IsColor),
                DocumentName = x.DocumentName,
                PagesPrinted = x.TotalPages,
                PaperKind = x.PaperKind,
                PaperLength = x.PaperLength,
                PaperWidth = x.PaperWidth,
                JobSize = x.JobSize,
                PrintProcessorName = x.PrintProcessorName,

                IsLandscape = Convert.ToBoolean(x.IsLandscape),
                IsDuplex = Convert.ToBoolean(x.IsDuplex),
                IsPrinted = Convert.ToBoolean(x.IsPrinted),
                IsCancelled = Convert.ToBoolean(x.IsCancelled),
                Signature = x.Signature,

                IdPrinter =  x.IdPrinter,
                IdDocument = x.IdDocument,
                IdUser = x.IdUser,
            }).ToList();
            return printerUsageLogsDTO.Envelope();
        }

        public async Task<ApiResultDTO<PagedResult<PrinterUsageLog>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex, string userId = "") {
            var query = _context.PrinterUsageLogs.OrderByDescending(x => x.UsageDate).AsQueryable();

            // filter user
            if (userId != "")
                query = query.Where(x => x.IdUser.Equals(new Guid(userId)));

            if (!keyword.Equals("empty")) {
                query = query.Where(x => x.PrinterName.Contains(keyword)
                 || x.DocumentName.Contains(keyword)
                 || x.UserName.Contains(keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<PrinterUsageLog>() {
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data
            };
            return new ApiResultDTO<PagedResult<PrinterUsageLog>>(true, "", pagedResult);
        }

        public async Task<ApiResultDTO<PrinterUsageLog>> GetByIdAsync(Guid id) {
            try {
                var printerUsageLog = await _context.PrinterUsageLogs.FindAsync(id);
                if (printerUsageLog == null) {
                    return new ApiResultDTO<PrinterUsageLog>(false, "Đối tượng không tồn tại");
                }
                return new ApiResultDTO<PrinterUsageLog>(true, "", printerUsageLog);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> InsertAsync(PrinterUsageLogDTO request) {
            //var printerJob = await _context.PrinterUsageLogs.FindAsync(request.Oid);
            //if (printerJob != null) {
            //    return new ApiResultDTO<bool>(false, "Đối tượng đã tồn tại", false);
            //}

            using var dbContextTransaction = _context.Database.BeginTransaction();
            try {
                var printer = await _context.Printers.FirstOrDefaultAsync(x => x.Name.Equals(request.PrinterName));
                var document = await _context.Documents.FirstOrDefaultAsync(x => x.Name.Equals(request.DocumentName));

                var user = await _context.Users.FindAsync(request.IdUser);

                var printerUsageLog = new PrinterUsageLog() {
                    UsageDate = request.UsageDate,
                    UsageBy = request.UsageBy,
                    //UsageCost = request.UsageCost,
                    MachineName = request.MachineName,
                    JobId = request.JobId,
                    UserName = request.UserName,
                    JobStatus = request.JobStatus,
                    Copies = request.Copies,
                    TotalPages = request.TotalPages,
                    PrinterName = request.PrinterName,
                    DriverName = request.DriverName,
                    IsColor = request.IsColor,
                    DocumentName = request.DocumentName,
                    PagesPrinted = request.TotalPages,
                    PaperKind = request.PaperKind,
                    PaperLength = request.PaperLength,
                    PaperWidth = request.PaperWidth,
                    JobSize = request.JobSize,
                    PrintProcessorName = request.PrintProcessorName,

                    IsLandscape = request.IsLandscape,
                    IsDuplex = request.IsDuplex,
                    IsPrinted = request.IsPrinted,
                    IsCancelled = request.IsCancelled,
                    

                    IdPrinter =  printer != null ? printer.Oid : null,
                    IdDocument = document != null ? document.Oid : null,
                    IdUser = request.IdUser,
                };

                if (!request.IsCancelled) {
                    if (printer != null) { // Cập nhật số liệu in của thiết bị máy in
                        printer.TotalJobs += 1;
                        printer.TotalPages += request.TotalPages;
                        printer.LastUsageDate = DateTime.Now;
                        printerUsageLog.UsageCost = printer.DefaultCost * printerUsageLog.TotalPages;
                        //printerUsageLog.UsageCost = Convert.ToBoolean(printer.IsNetwork) ? printer.DefaultCost * printerUsageLog.TotalPages : 0;
                        _context.Printers.Update(printer);


                        if (user != null) { // Cập nhật số liệu in của người dùng
                            user.TotalJobs += 1;
                            user.TotalPages += request.TotalPages;
                            user.RemainPages -=  request.TotalPages;
                            user.Balance = user.RemainPages * printer.DefaultCost;
                            //if (Convert.ToBoolean(printer.IsNetwork)) {
                            //    user.RemainPages -=  request.TotalPages;
                            //    user.Balance = user.RemainPages * printer.DefaultCost;
                            //}

                            _context.Users.Update(user);
                        }
                    }

                    // gán signature
                    printerUsageLog.Signature = request.Signature;
                }
                else
                    printerUsageLog.UsageCost = 0;

                await _context.PrinterUsageLogs.AddAsync(printerUsageLog);
                await _context.SaveChangesAsync();

                await dbContextTransaction.CommitAsync();
                return new ApiResultDTO<bool>(true, "Thêm thành công");
            }
            catch (Exception ex) {
                dbContextTransaction.Rollback();
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, PrinterUsageLogDTO request) {
            try {
                var printerUsageLog = await _context.PrinterUsageLogs.FindAsync(id);
                if (printerUsageLog == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                printerUsageLog.UsageDate = request.UsageDate;
                printerUsageLog.UsageBy = request.UsageBy;
                //printerUsageLog.UsageCost = request.UsageCost;
                printerUsageLog.MachineName = request.MachineName;
                printerUsageLog.JobId = request.JobId;
                printerUsageLog.UserName = request.UserName;
                printerUsageLog.JobStatus = request.JobStatus;
                printerUsageLog.Copies = request.Copies;
                printerUsageLog.TotalPages = request.TotalPages;
                printerUsageLog.PrinterName = request.PrinterName;
                printerUsageLog.DriverName = request.DriverName;
                printerUsageLog.IsColor = request.IsColor;
                printerUsageLog.DocumentName = request.DocumentName;
                printerUsageLog.PagesPrinted = request.TotalPages;
                printerUsageLog.PaperKind = request.PaperKind;
                printerUsageLog.PaperLength = request.PaperLength;
                printerUsageLog.PaperWidth = request.PaperWidth;
                printerUsageLog.JobSize = request.JobSize;
                printerUsageLog.PrintProcessorName = request.PrintProcessorName;

                printerUsageLog.IsLandscape = request.IsLandscape;
                printerUsageLog.IsDuplex = request.IsDuplex;
                printerUsageLog.IsPrinted = request.IsPrinted;
                printerUsageLog.IsCancelled = request.IsCancelled;
                printerUsageLog.Signature = request.Signature;

                printerUsageLog.IdPrinter = request.IdPrinter;
                printerUsageLog.IdDocument = request.IdDocument;
                printerUsageLog.IdUser = request.IdUser;

                _context.PrinterUsageLogs.Update(printerUsageLog);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
