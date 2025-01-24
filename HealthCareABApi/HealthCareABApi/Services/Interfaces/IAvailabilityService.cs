using HealthCareABApi.DTO;
using HealthCareABApi.Models;

namespace HealthCareABApi.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task CreateAvailabilityAsync(CreateAvailabilityDTO dto);
        Task<Availability> GetAvailabilityByCaregiverIdAsync(string caregiverId, DateTime? dateTime);
        Task UpdateAvailabilityByIdAsync(string id, UpdateAvailabilityDTO dto);
        Task DeleteAvailabilityByIdAsync(string id);
        Task<Availability> GetAvailabilityByIdAsync(string id);
        Task<List<Availability>> GetAllAvailabilitiesByDateAsync(DateTime date);
        Task<List<Availability>> GetAllAvailabilitiesAsync();
    }
}
