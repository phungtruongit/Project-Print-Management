namespace PrintManagement.ServiceAPI.RepositoryServices
{
    public class ResetUserRequest
    {
        public Guid Oid { get; set; }
        public string ResetBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
