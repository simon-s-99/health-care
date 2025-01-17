using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IAvailabilityService
    {
        Task CreateAvailabilityAsync(CreateAvailabilityDTO dto);
        Task<List<Availability>> GetAllAvailabilitiesByCaregiverIdAsync(string caregiverId);
        Task UpdateAvailabilityByIdAsync(string id, UpdateAvailabilityDTO dto);
        Task DeleteAvailabilityByIdAsync(string id);
        Task<Availability> GetAvailabilityByIdAsync(string id);
    }
}
