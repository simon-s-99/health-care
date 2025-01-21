using System;
using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(string id);
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(FilterDefinition<Appointment> filter, UpdateDefinition<Appointment> update);
        Task DeleteAsync(string id);
        Task<List<Appointment>> GetAllByPatientId(string id, DateTime? date);
        Task<List<Appointment>> GetAllByCaregiverId(string id, DateTime? date);

    }
}

