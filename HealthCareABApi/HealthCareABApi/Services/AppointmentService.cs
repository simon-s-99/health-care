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
        private readonly UserService _userService;
        private readonly IAvailabilityService _availabilityService;


        public AppointmentService(IOptions<MongoDBSettings> mongoDBSettings, UserService userService, IAvailabilityService availabilityService)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _appointments = database.GetCollection<Appointment>("Appointments");
            _userService = userService;
            _availabilityService = availabilityService;
        }

        public async Task CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            bool bothUsersExist = await _userService.ExistsByIdAsync(dto.PatientId) && await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!bothUsersExist)
            {
                throw new KeyNotFoundException("User(s) not found.");
            }

            var availability = await _availabilityService.GetAvailabilityStatusByCaregiverIdAndDateAsync(dto.CaregiverId, dto.DateTime) ?? throw new BadHttpRequestException("Caregiver is not available.");

            Appointment appointment = new Appointment
            {
                CaregiverId = dto.CaregiverId,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime.ToUniversalTime(),
                Status = dto.Status,
            };

            await _appointments.InsertOneAsync(appointment);

            UpdateAvailabilityDTO updatedAvailability = new UpdateAvailabilityDTO
            {
                AvailableSlots = availability.AvailableSlots
            };

            await _availabilityService.UpdateAvailabilityByIdAsync(availability.Id, updatedAvailability);
        }   

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointments.Find(u => u.Id == id).FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<List<Appointment>> GetAllAppointmentsByUserIdAsync(string id, DateTime? date, bool isPatient)
        {
            var appointments = new List<Appointment>();

            if (isPatient)
            {
                appointments = await _appointments.Find(u => u.PatientId == id).ToListAsync();
            } 
            else
            {
                appointments = await _appointments.Find(u => u.CaregiverId == id).ToListAsync();
            }

            if (date is not null)
            {
                appointments = appointments.FindAll(a => a.DateTime.Date == date.Value.Date);
            }

            return appointments.OrderBy(a => a.DateTime).ToList();
        }

        public async Task UpdateAppointmentByIdAsync(string id, UpdateAppointmentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var originalAppointment = await GetAppointmentByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            if (dto.DateTime is not null && dto.DateTime < DateTime.Today)
            {
                throw new BadHttpRequestException("Invalid date.");
            }

            // Get appointment by id
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);

            // Define updates to be made
            var updates = Builders<Appointment>.Update.Combine(
                Builders<Appointment>.Update.Set("DateTime", dto.DateTime ?? originalAppointment.DateTime), // If undefined, set them to their original values
                Builders<Appointment>.Update.Set("Status", dto.Status ?? originalAppointment.Status)
            );

            await _appointments.UpdateOneAsync(filter, updates);
        }

        public async Task DeleteAppointmentByIdAsync(string id)
        {
            var appointment = await GetAppointmentByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            // Get appointment by id
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, appointment.Id);

            await _appointments.DeleteOneAsync(filter);
        }
    }
}
