using System;

namespace PrintManagement.ApiIntegration {
    public class RoleAssignRequest {
        public Guid IdUser { get; set; }
        public string Name { get; set; }
        public Guid IdRole { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
