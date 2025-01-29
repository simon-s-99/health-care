using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using HealthCareABApi.Services.Interfaces;
using MongoDB.Driver;

namespace HealthCareABApi.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly IUserService _userService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IAvailabilityService availabilityService, IUserService userService)
        {
            _appointmentRepository = appointmentRepository;
            _availabilityService = availabilityService;
            _userService = userService;
        }

        /// <summary>
        /// Create an appointment for a user (patient).
        /// </summary>
        /// <param name="dto">The changes to be made.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If one or more users do not exist.</exception>
        /// <exception cref="BadHttpRequestException">If an available slot for the booking does not exist.</exception>
        public async Task CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            bool bothUsersExist = await _userService.ExistsByIdAsync(dto.PatientId) && await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!bothUsersExist)
            {
                throw new KeyNotFoundException("User(s) not found.");
            }

            var availability = await _availabilityService.GetAvailabilityByCaregiverIdAsync(dto.CaregiverId, dto.DateTime) ?? throw new BadHttpRequestException("Caregiver is not available.");

            Appointment appointment = new Appointment
            {
                CaregiverId = dto.CaregiverId,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Status = dto.Status,
            };

            await _appointmentRepository.CreateAsync(appointment);

            // Delete availability
            await _availabilityService.DeleteAvailabilityByIdAsync(availability.Id);
        }   

        /// <summary>
        /// Get all upcoming and previous appointments for a specific user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <param name="date">(Optional) Get appointments for a specific date only.</param>
        /// <param name="isPatient">Whether to search for the patientId or caregiverId.</param>
        /// <returns>A list of appointments, or an empty list.</returns>
        public async Task<List<Appointment>> GetAllAppointmentsByUserIdAsync(string id, DateTime? date, bool isPatient)
        {
            var appointments = new List<Appointment>();

            if (isPatient)
            {
                appointments = await _appointmentRepository.GetAllByPatientId(id, date ?? null);
            } 
            else
            {
                appointments = await _appointmentRepository.GetAllByCaregiverId(id, date ?? null);
            }

            return appointments.OrderBy(a => a.DateTime).ToList(); //Returns empty array if user is valid but no appointments are found
        }

        /// <summary>
        /// Get an appointment by its id.
        /// </summary>
        /// <param name="id">The id of the appointment.</param>
        /// <returns>The appointment, or nothing.</returns>
        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment;
        }

        /// <summary>
        /// Updates an appointment with the specified values.
        /// </summary>
        /// <param name="id">The id of the appojntment to be updated.</param>
        /// <param name="dto">The changes to be made.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If appointment doesn't exist.</exception>
        /// <exception cref="BadHttpRequestException">If the date is invalid, if supplied.</exception>
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

        /// <summary>
        /// Delete an appointment by its id.
        /// </summary>
        /// <param name="id">The id of the appointment.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If appointment does not exist.</exception>
        public async Task DeleteAppointmentByIdAsync(string id)
        {
            var appointment = await GetAppointmentByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");

            await _appointmentRepository.DeleteAsync(id);

            var previousAvailability = await _availabilityService.GetAvailabilityByCaregiverIdAsync(appointment.CaregiverId, null);

            CreateAvailabilityDTO availabilityDto = new CreateAvailabilityDTO(appointment.CaregiverId, appointment.DateTime);

            // Create a new availability with the canceled appointment's date, time, and caregiver
            await _availabilityService.CreateAvailabilityAsync(availabilityDto);
        }
    }
}
