namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class ForgetPasswordRequest {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDisable { get; set; }
        public bool IsRestricted { get; set; }
        public double? Balance { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? IdUserGroup { get; set; }
        public Guid? IdDepartment { get; set; }
    }
}
