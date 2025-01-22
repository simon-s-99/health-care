using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthCareABApi.Services;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO dto)
        {
            if (string.IsNullOrEmpty(dto.PatientId) || string.IsNullOrEmpty(dto.CaregiverId) || dto.DateTime < DateTime.Today || dto.Status == AppointmentStatus.None)
            {
                return BadRequest("One more more fields are null.");
            }

            try
            {
                await _appointmentService.CreateAppointmentAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("id")]
        public async Task<IActionResult> GetAppointmentById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid appointment id.");
            }

            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (appointment == null)
                {
                    return NotFound(); 
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindFirst(ClaimTypes.Role)?.Value;

                bool isUserUnauthorized =
                    (roles?.Contains(Roles.User) == true && appointment.PatientId != userId) ||
                    (roles?.Contains(Roles.Admin) == true && appointment.CaregiverId != userId);

                if (isUserUnauthorized)
                {
                    return Forbid("You do not have access to this appointment.");
                }

                return Ok(appointment);
            }
            catch (FormatException)
            {
                return NotFound(); 
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        /// <summary>
        /// Retrieves appointments for a specific user, optionally filtered by role and date.
        /// </summary>
        /// <param name="id">The user's ID (Patient or Caregiver).</param>
        /// <param name="date">Optional filter for appointments on a specific date.</param>
        /// <param name="isPatient">Whether the user is a patient (default is true).</param>
        /// <returns>A list of appointments matching the criteria.</returns>
        [HttpGet("user/")]
        public async Task<IActionResult> GetAllAppointmentsByUserIdAsync([FromQuery] string id, [FromQuery] bool isPatient = true) // Defaults to true
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsByUserIdAsync(id, isPatient);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAppointmentById([FromQuery] string id, [FromBody] UpdateAppointmentDTO dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                await _appointmentService.UpdateAppointmentByIdAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAppointmentById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                await _appointmentService.DeleteAppointmentByIdAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
