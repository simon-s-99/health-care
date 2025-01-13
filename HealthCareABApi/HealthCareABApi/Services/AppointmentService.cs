using HealthCareABApi.Configurations;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HealthCareABApi.Services
{
    public class AppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _appointments = database.GetCollection<Appointment>("Appointments");
        }

        public async Task CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

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

            // TODO: Add logic to check that patient and caregiver actually exist
            await _appointments.InsertOneAsync(appointment);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointments.Find(u => u.Id == id).FirstOrDefaultAsync();

            ArgumentNullException.ThrowIfNull(appointment);

            return appointment;
        }

        public async Task UpdateAppointmentByIdAsync(string id, UpdateAppointmentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

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
            await _appointments.DeleteOneAsync(id);
        }
    }
}
