using HealthCareABApi.Configurations;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HealthCareABApi.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IMongoCollection<Availability> _availabilities;
        private readonly UserService _userService;

        public AvailabilityService(IOptions<MongoDBSettings> mongoDBSettings, UserService userService)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _availabilities = database.GetCollection<Availability>("Availabilities");
            _userService = userService;
        }

        public async Task CreateAvailabilityAsync(CreateAvailabilityDTO dto)
        {
            bool userExists = await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!userExists)
            {
                throw new KeyNotFoundException("User not found.");
            }

            Availability availability = new Availability
            {
                CaregiverId = dto.CaregiverId,
                AvailableSlots = dto.AvailableSlots,
            };

            await _availabilities.InsertOneAsync(availability);
        }

        public async Task DeleteAvailabilityByIdAsync(string id)
        {
            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            // Get availability by id
            var filter = Builders<Availability>.Filter.Eq(a => a.Id, availability.Id);

            await _availabilities.DeleteOneAsync(filter);
        }

        public async Task<List<Availability>> GetAllAvailabilitiesByCaregiverIdAsync(string caregiverId)
        {
            var availabilities = await _availabilities.Find(u => u.CaregiverId == caregiverId).ToListAsync();
            return availabilities;
        }

        public async Task<Availability> GetAvailabilityByIdAsync(string id)
        {
            var appointment = await _availabilities.Find(u => u.Id == id).FirstOrDefaultAsync();
            return appointment;
        }

        public async Task UpdateAvailabilityByIdAsync(string id, UpdateAvailabilityDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            // Get availability by id
            var filter = Builders<Availability>.Filter.Eq(a => a.Id, id);

            // Define update to be made
            var update = Builders<Availability>.Update.Set("AvailableSlots", dto.AvailableSlots);

            await _availabilities.UpdateOneAsync(filter, update);
        }
    }
}
