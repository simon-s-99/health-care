using System;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<List<Availability>> GetAllAsync();
        Task<Availability> GetByIdAsync(string id);
        Task CreateAsync(Availability availability);
        Task UpdateAsync(string id, Availability availability);
        Task DeleteAsync(string id);
        Task<List<Availability>> GetByCaregiverIdAsync(string caregiverId);

    }
}

