using HealthCareABApi.Configurations;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HealthCareABApi.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;
        private readonly IUserService _userService;

        public AppointmentService(IOptions<MongoDBSettings> mongoDBSettings, IUserService userService)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _appointments = database.GetCollection<Appointment>("Appointments");
            _userService = userService;
        }

        public async Task CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            bool bothUsersExist = await _userService.ExistsByIdAsync(dto.PatientId) && await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!bothUsersExist)
            {
                throw new KeyNotFoundException("User(s) not found.");
            }

            if (dto.DateTime < DateTime.Today)
            {
                throw new BadHttpRequestException("Invalid date.");
            }

            Appointment appointment = new Appointment
            {
                CaregiverId = dto.CaregiverId,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Status = dto.Status,
            };

            await _appointments.InsertOneAsync(appointment);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointments.Find(u => u.Id == id).FirstOrDefaultAsync();
            return appointment;
        }

        public async Task UpdateAppointmentByIdAsync(string id, UpdateAppointmentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            _ = await GetAppointmentByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            if (dto.DateTime < DateTime.Today)
            {
                throw new BadHttpRequestException("Invalid date.");
            }

            // Get appointment by id
            var filter = Builders<Appointment>.Filter.Eq("id", id);

            // Define updates to be made
            var updates = Builders<Appointment>.Update.Combine(
                Builders<Appointment>.Update.Set("DateTime", dto.DateTime),
                Builders<Appointment>.Update.Set("Status", dto.Status)
            );

            await _appointments.UpdateOneAsync(filter, updates);
        }

        public async Task DeleteAppointmentByIdAsync(string id)
        {
            var appointment = await GetAppointmentByIdAsync(id);

            if (appointment is null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            await _appointments.DeleteOneAsync(id);

        }
    }
}
