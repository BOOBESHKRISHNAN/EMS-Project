

namespace EventManagementCL.DTO
{
    public class RegisterAdminRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Location { get; set; } // Required for POCs
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
