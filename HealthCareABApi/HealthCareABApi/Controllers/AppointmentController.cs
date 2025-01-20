using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareABApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> GetAppointmentById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid id.");
            }

            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (appointment is null)
                {
                    return NotFound();
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
