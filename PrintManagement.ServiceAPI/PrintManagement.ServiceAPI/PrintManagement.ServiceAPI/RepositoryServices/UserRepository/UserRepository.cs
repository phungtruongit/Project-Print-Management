using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.Common;
using PrintManagement.ServiceAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class UserRepository : IUserRepository {
        private readonly AppSettings _appSettings;
        private readonly PrintManagementContext _context;

        public UserRepository(IOptionsMonitor<AppSettings> optionsMonitor, PrintManagementContext context) {
            _appSettings = optionsMonitor.CurrentValue;
            _context = context;
        }

        public async Task<ApiResultDTO<string>> LoginAsync(LoginRequest loginModel) {
            try {
                var user = await _context.Users.FirstOrDefaultAsync(p => p.UserName.Equals(loginModel.UserName));
                if (user == null)
                    return new ApiResultDTO<string>(false, "Tài khoản không tồn tại!");
                if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
                    return new ApiResultDTO<string>(false, "Mật khẩu không chính xác!");

                var token = await AuthenticateAsync(user);

                return new ApiResultDTO<string>(true, "Đăng nhập thành công!", token);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<string> AuthenticateAsync(User user) {
            try {
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secretkey);

                var subject = new ClaimsIdentity(
                        new[] {
                        new Claim(ClaimTypes.Name,user.Name),
                        new Claim(ClaimTypes.Email,user.Email),
                        new Claim("Phone",user.Phone),
                        new Claim("UserName",user.UserName),
                        new Claim("UserId",user.Oid.ToString()),
                        new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        });

                // Add Roles
                var roles = _context.UserRoles.Where(x => x.IdUser.Equals(user.Oid));
                foreach (var item in roles) {
                    var role = await _context.Roles.FindAsync(item.IdRole);
                    if (role != null)
                        subject.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                }

                var tokenDescription = new SecurityTokenDescriptor {
                    Subject = subject,
                    Expires = DateTime.UtcNow.AddHours(12),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature),
                };

                var token = jwtTokenHandler.CreateToken(tokenDescription);
                return jwtTokenHandler.WriteToken(token);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<List<User>>> GetAllAsync() {
            try {
                var users = await _context.Users.OrderBy(x => x.CreatedDate).ToListAsync();
                return new ApiResultDTO<List<User>>(true, "", users);
            }
            catch (Exception ex) {
                throw;
            }
        }
        public async Task<ApiResultDTO<PagedResult<User>>> GetAllPagingAsync(string keyword, int pageSize, int pageIndex) {
            var query = _context.Users.OrderBy(x => x.CreatedDate).AsQueryable();

            if (!keyword.Equals("empty")) {
                query = query.Where(x => x.UserName.Contains(keyword)
                 || x.Name.Contains(keyword) || x.Phone.Contains(keyword) || x.Email.Contains(keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            // Select and projection
            var pagedResult = new PagedResult<User>() {
                TotalRecords = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data
            };
            return new ApiResultDTO<PagedResult<User>>(true, "", pagedResult);
        }

        public async Task<ApiResultDTO<bool>> RegisterAsync(RegisterRequest request) {
            using var transaction = _context.Database.BeginTransaction();
            try {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.Equals(request.UserName));
                if (user != null) {
                    return new ApiResultDTO<bool>(false, "Tài khoản đã tồn tại");
                }
                var existEmail = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(request.Email));
                if (existEmail != null) {
                    return new ApiResultDTO<bool>(false, "Emai đã tồn tại");
                }
                var idUser = Guid.NewGuid();
                user = new User() {
                    Oid = idUser,
                    Name = request.Name,
                    Phone = request.Phone,
                    Email = request.Email,
                    UserName = request.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    IsAdmin = request.IsAdmin,
                    IsDisable = request.IsDisable,
                    IsRestricted = request.IsRestricted,
                    Balance = request.Balance,
                    RemainPages = request.RemainPages,

                    TotalJobs = 0,
                    TotalPages = 0,
                    Note = request.Note,
                    CreatedBy = request.CreatedBy,
                    CreatedDate = request.CreatedDate,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedDate = request.ModifiedDate,
                    IdDepartment = request.IdDepartment,
                    IdUserGroup = request.IdUserGroup,
                };

                var userRole = new UserRole();
                userRole.Oid = Guid.NewGuid();
                userRole.IdUser = idUser;
                userRole.IdRole = new Guid("78f76bf2-11cd-4b1d-8546-12f22cd4b59d"); // Nhân viên
                if (request.IsAdmin) {
                    userRole.IdRole = new Guid("def25f45-b2a0-426b-a631-f19cc73f4603"); // Quyền quản trị viên
                }

                await _context.Users.AddAsync(user);
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return new ApiResultDTO<bool>(true, "Đăng ký thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> DeleteAsync(Guid id) {
            try {
                var user = await _context.Users.FindAsync(id);
                if (user == null) {
                    return new ApiResultDTO<bool>(false, "User không tồn tại");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new ApiResultDTO<bool>(true, "Xoá User thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<User>> GetByIdAsync(Guid Id) {
            try {
                var user = await _context.Users.FindAsync(Id);
                if (user == null) {
                    return new ApiResultDTO<User>(false, "User không tồn tại");
                }
                return new ApiResultDTO<User>(true, "", user);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> ResetUserAsync(ResetUserRequest request) {
            try {
                var user = await _context.Users.FindAsync(request.Oid);
                if (user == null) {
                    return new ApiResultDTO<bool>(false, "User không tồn tại");
                }

                var userConfig = await _context.UserConfigs.FirstOrDefaultAsync();
                var financilConfig = await _context.FinancialConfigs.FirstOrDefaultAsync();

                user.TotalJobs = 0;
                user.TotalPages = 0;
                user.Balance = userConfig.DefaultBalance;
                user.RemainPages = userConfig.DefaultBalance / financilConfig.DefaultPrintCost;
                user.ResetBy = request.ResetBy;
                user.ResetDate = DateTime.Now;
                user.ModifiedBy = request.ModifiedBy;
                user.ModifiedDate = DateTime.Now;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Reset user thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> UpdateAsync(Guid id, UserDTO request) {
            try {
                var user = await _context.Users.FindAsync(id);
                if (user == null) {
                    return new ApiResultDTO<bool>(false, "Đối tượng không tồn tại");
                }

                user.Name = request.Name;
                user.Phone = request.Phone;
                user.Email = request.Email;
                user.UserName = request.UserName;
                user.Password = request.Password;
                user.IsAdmin = request.IsAdmin;
                user.IsDisable = request.IsDisable;
                user.IsRestricted = request.IsRestricted;
                user.Balance = request.Balance;
                user.RemainPages = request.RemainPages;
                user.Note = request.Note;
                user.IdDepartment = request.IdDepartment;
                user.IdUserGroup = request.IdUserGroup;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Sửa thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> ChangePasswordAsync(ChangePasswordRequest request) {
            try {
                var user = await _context.Users.FindAsync(request.Oid);
                if (user == null) {
                    return new ApiResultDTO<bool>(false, "Tài khoản không tồn tại");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.OldPass, user.Password)) {
                    return new ApiResultDTO<bool>(false, "Mật khẩu cũ không đúng");
                }
                else {
                    if (BCrypt.Net.BCrypt.Verify(request.NewPass, user.Password)) {
                        return new ApiResultDTO<bool>(false, "Mật khẩu mới phải khác mật khẩu cũ");
                    }
                    else {
                        if (!request.NewPass.Equals(request.Confirm)) {
                            return new ApiResultDTO<bool>(false, "Mật khẩu mới và xác nhận mật khẩu không khớp");
                        }
                    }
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPass);
                user.ModifiedBy = request.ModifiedBy;
                user.CreatedDate = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new ApiResultDTO<bool>(true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<ApiResultDTO<bool>> ForgetPasswordAsync(ForgetPasswordRequest request) {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckPrintPermissionAsync(Guid idUser, int totalPage, string printerFullName) {
            var user = await _context.Users.FindAsync(idUser);
            var printer = await _context.Printers.FirstOrDefaultAsync(x => x.Name.Equals(printerFullName));

            if (user == null) {
                return false;
            }
            if (printer == null) {
                return false;
            }

            if (Convert.ToInt32(user.RemainPages) < totalPage)
                return false;

            if (printer.IdDepartment == null) { // Máy in k thuộc phòng ban nào! => chưa dc phân quyền => cho phép in
                return true;
            }

            if (!printer.IdDepartment.Equals(user.IdDepartment)) { // Check department of user và printer
                return false;
            }
            else
                return true;
        }


        public async Task<ApiResultDTO<bool>> RoleAssignAsync(RoleAssignRequest roleAssignRequest) {
            var user = await _context.Users.FindAsync(roleAssignRequest.IdUser);
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.IdUser.Equals(roleAssignRequest.IdUser) && x.IdRole.Equals(roleAssignRequest.IdRole));

            if (user == null) return new ApiResultDTO<bool>(false, "Người dùng không tồn tại!");
            //if (userRole == null) return new ApiResultDTO<bool>(false, "Người dùng chưa được phân quyền");

            if (userRole == null) {
                userRole = new UserRole() {
                    Oid = Guid.NewGuid(),
                    IdRole = roleAssignRequest.IdRole,
                    IdUser = roleAssignRequest.IdUser,
                    Status = "active",
                };

                await _context.UserRoles.AddAsync(userRole);
            }
            else {
                userRole.IdUser = roleAssignRequest.IdUser;
                userRole.IdRole = roleAssignRequest.IdRole;
                _context.UserRoles.Update(userRole);
            }

            user.ModifiedBy = roleAssignRequest.ModifiedBy;
            user.ModifiedBy = roleAssignRequest.ModifiedBy;

            _context.Update(user);

            await _context.SaveChangesAsync();
            return new ApiResultDTO<bool>(true, String.Empty, true);
        }

        public async Task<ApiResultDTO<Guid>> GetUserRoleAsync(Guid idUser) {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.IdUser.Equals(idUser));
            if (userRole == null || userRole.IdRole == null) {
                return new ApiResultDTO<Guid>(false, "Người dùng chưa được phân quyền");
            }
            return new ApiResultDTO<Guid>(true, String.Empty, (Guid)userRole.IdRole);
        }

        public async Task<ApiResultDTO<List<Role>>> GetRolesAsync() {
            var roles = await _context.Roles.ToListAsync();

            return new ApiResultDTO<List<Role>>(true, String.Empty, roles);
        }
    }
}
