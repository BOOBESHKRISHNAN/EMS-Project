
namespace EventManagementCL.Models
{
    public enum UserRole
    {
        SuperAdmin,
        Admin,
        Organizer,
        RegisteredUser
    }

    public class User
    {
        public int Id { get; set; }

        // Basic Info
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Contact & Credentials
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? PasswordHash { get; set; } // Store hashed password, not plaintext

        public string ? Location { get; set; }

        // Role Assignment
        public UserRole Role { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
