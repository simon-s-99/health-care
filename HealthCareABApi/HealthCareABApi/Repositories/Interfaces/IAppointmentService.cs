using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IAppointmentService
    {
        Task CreateAppointmentAsync(CreateAppointmentDTO dto);
        Task<Appointment> GetAppointmentByIdAsync(string id);
        Task UpdateAppointmentByIdAsync(string id, UpdateAppointmentDTO dto);
        Task DeleteAppointmentByIdAsync(string id);
        Task<List<Appointment>> GetAllAppointmentsByUserIdAsync(string id, DateTime? date, bool isPatient);
    }
}