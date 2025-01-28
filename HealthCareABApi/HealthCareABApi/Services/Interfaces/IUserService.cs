using HealthCareABApi.Models;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(string id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        string HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string storedHash);
    }
}

