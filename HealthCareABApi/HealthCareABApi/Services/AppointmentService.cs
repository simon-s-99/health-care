using HealthCareABApi.Configurations;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Implementations;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HealthCareABApi.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly UserService _userService;
        private readonly IAvailabilityService _availabilityService;

        public AppointmentService(IAppointmentRepository appointmentRepository, UserService userService)
        {
            _appointmentRepository = appointmentRepository;
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
            updatedAvailability.AvailableSlots.Remove(dto.DateTime);

            await _availabilityService.UpdateAvailabilityByIdAsync(availability.Id, updatedAvailability);
        }   

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment;
        }

        public async Task<List<Appointment>> GetAllAppointmentsByUserIdAsync(string id, DateTime? date, bool isPatient)
        {
            var appointments = new List<Appointment>();

            if (isPatient)
            {
                appointments = await _appointmentRepository.GetAllByPatientId(id);
            } 
            else
            {
                appointments = await _appointmentRepository.GetAllByCaregiverId(id);
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

            await _appointmentRepository.UpdateAsync(filter, updates);
        }

        public async Task DeleteAppointmentByIdAsync(string id)
        {
            var appointment = await GetAppointmentByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");
            await _appointmentRepository.DeleteAsync(id);
        }
    }
}
