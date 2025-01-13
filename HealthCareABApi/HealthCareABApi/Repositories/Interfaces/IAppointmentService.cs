using HealthCareABApi.DTO;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IAppointmentService
    {
        Task CreateAppointmentAsync(CreateAppointmentDTO dto);
        Task<Appointment> GetAppointmentByIdAsync(string id);
        Task UpdateAppointmentByIdAsync(string id, UpdateAppointmentDTO dto);
        Task DeleteAppointmentByIdAsync(string id);
    }
}