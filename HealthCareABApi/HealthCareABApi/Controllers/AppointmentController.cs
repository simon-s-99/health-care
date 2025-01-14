﻿using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using HealthCareABApi.Services;
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
