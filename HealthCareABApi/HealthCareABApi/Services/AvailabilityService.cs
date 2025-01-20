using HealthCareABApi.Configurations;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

namespace HealthCareABApi.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly UserService _userService;

        public AvailabilityService(IAvailabilityRepository availabilityRepository, UserService userService)
        {
            _availabilityRepository = availabilityRepository;
            _userService = userService;
        }

        public async Task CreateAvailabilityAsync(CreateAvailabilityDTO dto)
        {
            bool userExists = await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!userExists)
            {
                throw new KeyNotFoundException("User not found.");
            }

            foreach (DateTime timeSlot in dto.AvailableSlots)
            {
                if (timeSlot < DateTime.Now)
                {
                    throw new KeyNotFoundException("Invalid date.");
                }
            }

            var existingAvailbility = await GetAvailabilityStatusByCaregiverIdAndDateAsync(dto.CaregiverId, null);

            if (existingAvailbility is not null)
            {
                UpdateAvailabilityDTO updated = new UpdateAvailabilityDTO
                {
                    AvailableSlots = existingAvailbility.AvailableSlots.Concat(dto.AvailableSlots).ToList()
                };
                var availableSlotsWithoutDuplicates = updated.AvailableSlots.Distinct().ToList();
                updated.AvailableSlots = availableSlotsWithoutDuplicates;

                await UpdateAvailabilityByIdAsync(existingAvailbility.Id, updated);
            }
            else
            {
                Availability availability = new Availability
                {
                    CaregiverId = dto.CaregiverId,
                    AvailableSlots = dto.AvailableSlots,
                };

                await _availabilityRepository.CreateAsync(availability);
            }

        }

        public async Task DeleteAvailabilityByIdAsync(string id)
        {
            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");
            await _availabilityRepository.DeleteAsync(id);
        }

        public async Task<Availability> GetAvailabilityStatusByCaregiverIdAndDateAsync(string caregiverId, DateTime? dateTime)
        {
            var availability = new Availability();

            if (dateTime is not null)
            {
                availability = await _availabilityRepository.GetByCaregiverIdAndDate(caregiverId, (DateTime)dateTime);
            }
            else
            {
                availability = await _availabilityRepository.GetByCaregiverIdAsync(caregiverId);
            }

            return availability;

        }

        public async Task<List<Availability>> GetAllAvailabilitiesAsync()
        {
            return await _availabilityRepository.GetAllAsync();
        }

        public async Task<List<Availability>> GetAllAvailabilitiesByDateAsync(DateTime date)
        {
            var allAvailabilities = await GetAllAvailabilitiesAsync();

            List<Availability> result = new List<Availability>();

            foreach (var availability in allAvailabilities)
            {
                foreach (var slot in availability.AvailableSlots)
                {
                    if (slot.ToString().Split(" ")[0] == date.ToString().Split(" ")[0])
                    {
                        result.Add(availability);
                    }
                }
            }
            return result;
        }

        public async Task<Availability> GetAvailabilityByIdAsync(string id)
        {
            var appointment = await _availabilityRepository.GetByIdAsync(id);
            return appointment;
        }

        public async Task UpdateAvailabilityByIdAsync(string id, UpdateAvailabilityDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Availability not found.");

            var dtoWithoutDuplicates = dto.AvailableSlots.Distinct().ToList();

            // Get availability by id
            var filter = Builders<Availability>.Filter.Eq(a => a.Id, id);

            // Define update to be made
            var update = Builders<Availability>.Update.Set("AvailableSlots", dtoWithoutDuplicates);

            await _availabilityRepository.UpdateAsync(filter, update);
        }
    }
}
