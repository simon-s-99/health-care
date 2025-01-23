using MongoDB.Driver;
using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<List<Availability>> GetAllAsync();
        Task<Availability> GetByIdAsync(string id);
        Task CreateAsync(Availability availability);
        Task UpdateAsync(FilterDefinition<Availability> filter, UpdateDefinition<Availability> update);
        Task DeleteAsync(string id);
        Task<Availability> GetByCaregiverIdAsync(string caregiverId);
        Task<Availability> GetByCaregiverIdAndDate(string caregiverId, DateTime dateTime);

    }
}

