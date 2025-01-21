using HealthCareABApi.Models;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<IEnumerable<Availability>> GetAllAsync();
        Task<Availability> GetByIdAsync(string id);
        Task CreateAsync(Availability availability);
        Task UpdateAsync(string id, Availability availability);
        Task DeleteAsync(string id);
        Task<IEnumerable<Availability>> GetByCaregiverIdAsync(string caregiverId);

    }
}

