using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using HealthCareABApi.Services.Interfaces;
using MongoDB.Driver;

namespace HealthCareABApi.Services.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IUserService _userService;

        public AvailabilityService(IAvailabilityRepository availabilityRepository, IUserService userService)
        {
            _availabilityRepository = availabilityRepository;
            _userService = userService;
        }

        /// <summary>
        /// Create an availability for a user (caregiver).
        /// </summary>
        /// <param name="dto">The changes to be made.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If user does not exist.</exception>
        /// <exception cref="ArithmeticException">If any date is invalid.</exception>
        public async Task CreateAvailabilityAsync(CreateAvailabilityDTO dto)
        {
            bool userExists = await _userService.ExistsByIdAsync(dto.CaregiverId);

            if (!userExists)
            {
                throw new KeyNotFoundException("User not found.");
            }


            if (dto.DateTime < DateTime.Now.ToUniversalTime())
            {
                throw new ArithmeticException("Invalid date.");
            }

            var duplicateAvailability = await GetAvailabilityByCaregiverIdAsync(dto.CaregiverId, dto.DateTime);

            if (duplicateAvailability is not null)
            {
                throw new HttpRequestException("Availability already exists.");
            }

            Availability availability = new Availability
            {
                CaregiverId = dto.CaregiverId,
                DateTime = dto.DateTime,
            };

            await _availabilityRepository.CreateAsync(availability);
        }

        /// <summary>
        /// Delete an availability by its id.
        /// </summary>
        /// <param name="id">The id of the availability.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If the availability does not exist.</exception>
        public async Task DeleteAvailabilityByIdAsync(string id)
        {
            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Appointment not found.");
            await _availabilityRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Get an availability by its caregiver's id.
        /// </summary>
        /// <param name="caregiverId">The id of the caregiver.</param>
        /// <param name="dateTime">(Optional) The specific date of the availability.</param>
        /// <returns>An availability, or nothing.</returns>
        public async Task<Availability> GetAvailabilityByCaregiverIdAsync(string caregiverId, DateTime? dateTime)
        {
            if (dateTime is not null)
            {
                return await _availabilityRepository.GetByCaregiverIdAndDate(caregiverId, (DateTime)dateTime);
            }
            else
            {
                return await _availabilityRepository.GetByCaregiverIdAsync(caregiverId);
            }
        }

        /// <summary>
        /// Get all availabilites.
        /// </summary>
        /// <returns>A list of availabilites, or an empty list.</returns>
        public async Task<List<Availability>> GetAllAvailabilitiesAsync()
        {
            var allAvailabilites = await _availabilityRepository.GetAllAsync();
            return allAvailabilites.OrderBy(a => a.DateTime).ToList();
        }

        /// <summary>
        /// Get all availabilites from a specific date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>A list of availabilites, or an empty list.</returns>
        public async Task<List<Availability>> GetAllAvailabilitiesByDateAsync(DateTime date)
        {
            var allAvailabilities = await GetAllAvailabilitiesAsync();

            List<Availability> result = new List<Availability>();

            foreach (var availability in allAvailabilities)
            {
                if (availability.DateTime.ToString().Split(" ")[0] == date.ToString().Split(" ")[0]) // Compare only the date part of the DateTime
                {
                    result.Add(availability);
                }
            }
            return result;
        }

        /// <summary>
        /// Get an availability by its id.
        /// </summary>
        /// <param name="id">The id of the availability.</param>
        /// <returns>The availability or nothing.</returns>
        public async Task<Availability> GetAvailabilityByIdAsync(string id)
        {
            var appointment = await _availabilityRepository.GetByIdAsync(id);
            return appointment;
        }


        /// <summary>
        /// Update an availability by its id.
        /// </summary>
        /// <param name="id">The id of the availability.</param>
        /// <param name="dto">The changes to be made.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="KeyNotFoundException">If the availability does not exist.</exception>
        public async Task UpdateAvailabilityByIdAsync(string id, UpdateAvailabilityDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var availability = await GetAvailabilityByIdAsync(id) ?? throw new KeyNotFoundException("Availability not found.");

            // Get availability by id
            var filter = Builders<Availability>.Filter.Eq(a => a.Id, id);

            // Define update to be made
            var update = Builders<Availability>.Update.Set("DateTime", dto.DateTime);

            await _availabilityRepository.UpdateAsync(filter, update);
        }
    }
}
