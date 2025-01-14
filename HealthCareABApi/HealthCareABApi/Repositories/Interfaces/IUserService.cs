using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IUserService
    {
        Task<bool> ExistsByUsernameAsync(string username);

        Task<bool> ExistsByIdAsync(string id);

        Task<User> GetUserByUsernameAsync(string username);

        Task CreateUserAsync(User user);

        string HashPassword(string password);

        bool VerifyPassword(string enteredPassword, string storedHash);
    }
}
