using MongoDB.Driver;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(string id);
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(FilterDefinition<Appointment> filter, UpdateDefinition<Appointment> update);
        Task DeleteAsync(string id);
        Task<List<Appointment>> GetAllByPatientId(string id, DateTime? date);
        Task<List<Appointment>> GetAllByCaregiverId(string id, DateTime? date);

    }
}

