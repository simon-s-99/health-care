using HealthCareABApi.Models;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IAppointmentAvailabilityService
    {
        Task<List<Appointment>> GetAllAppointmentsByUserIdAsync(string id, DateTime? date, bool isPatient);
    }
}
