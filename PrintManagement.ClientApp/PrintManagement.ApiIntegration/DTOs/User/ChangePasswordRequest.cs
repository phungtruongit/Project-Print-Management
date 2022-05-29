using System;

namespace PrintManagement.ApiIntegration {
    public class ChangePasswordRequest {
        public Guid Oid { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string Confirm{ get; set; }
        public string ModifiedBy { get; set; }
    }
}
