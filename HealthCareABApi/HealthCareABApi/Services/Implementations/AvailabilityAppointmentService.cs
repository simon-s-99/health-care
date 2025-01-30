using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using HealthCareABApi.Services.Interfaces;
using MongoDB.Driver;

namespace HealthCareABApi.Services.Implementations
{
    public class AppointmentAvailabilityService : IAppointmentAvailabilityService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentAvailabilityService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
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

    }
}
