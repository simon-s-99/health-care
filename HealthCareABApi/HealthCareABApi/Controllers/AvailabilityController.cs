using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityDTO dto)
        {
            if (dto.AvailableSlots.Count == 0 || string.IsNullOrEmpty(dto.CaregiverId))
            {
                return BadRequest("One more more fields are null.");
            }

            try
            {
                await _availabilityService.CreateAvailabilityAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailabilityById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                var availability = await _availabilityService.GetAvailabilityByIdAsync(id);

                if (availability is null)
                {
                    return NotFound();
                }

                return Ok(availability);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetAllAvailabilitiesByDate([FromQuery] DateTime date)
        {
            if (date < DateTime.Today)
            {
                return BadRequest("Invalid date.");
            }

            try
            {
                var availabilities = await _availabilityService.GetAllAvailabilitiesByDateAsync(date);
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAvailabilityById([FromQuery] string id, [FromBody] UpdateAvailabilityDTO dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                await _availabilityService.UpdateAvailabilityByIdAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAvailabilityById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                await _availabilityService.DeleteAvailabilityByIdAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
