using HealthCareABApi.Configurations;
using HealthCareABApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HealthCareABApi.Services.Implementations
{

    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).AnyAsync();
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).AnyAsync();
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return await _users.Find(u => u.Phonenumber == phoneNumber).AnyAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            // The "filter" identifies which document in the "Users" collection to update.
            // It uses Builders<User>.Filter to match the user's Id

            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update
                .Set(u => u.Firstname, user.Firstname)
                .Set(u => u.Lastname, user.Lastname)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Phonenumber, user.Phonenumber)
                .Set(u => u.Username, user.Username)
                .Set(u => u.PasswordHash, user.PasswordHash);

            var result = await _users.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception("User not found");
            }
        }


        // Method to hash a plaintext password using BCrypt.
        public string HashPassword(string password)
        {
            // Hash the password and return the hashed string.
            // BCrypt automatically generates a salt and applies it to the password, adding strong security to the hash.
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method to verify a plaintext password against a hashed password.
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Check if the entered password, when hashed, matches the stored hash.
            // BCrypt compares the entered password with the hashed password and returns true if they match.
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }
    }
}