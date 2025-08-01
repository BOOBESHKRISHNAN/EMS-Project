using EventManagementCL.Models;


namespace EventManagementCL.Interface
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        // Add registration or CRUD methods as needed later

        Task AddUserAsync(User user);


    }
}
