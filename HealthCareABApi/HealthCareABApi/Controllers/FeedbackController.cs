using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthCareABApi.Services;
using HealthCareABApi.Repositories.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IAppointmentService _appointmentService;

    // Constructor to inject the feedback repository
    public FeedbackController(IFeedbackRepository feedbackRepository, IAppointmentService appointmentService)
    {
        _feedbackRepository = feedbackRepository;
        _appointmentService = appointmentService;       
    }


    [HttpGet]
    public async Task<IActionResult> GetFeedback([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page and PageSize must be greater than zero.");
        }

        try
        {

            // Fetch feedback from the database in small chunks (pages) from the repository instead of all at once.
            // This prevents the system from running out of memory when the number of feedbacks grows.
            var paginatedFeedback = await _feedbackRepository.GetPaginatedFeedbackAsync(page, pageSize);

            var feedbackList = paginatedFeedback.ToList();
            
            return Ok(feedbackList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching feedback: {ex.Message}");
            return StatusCode(500, "An error occurred while fetching feedback.");
        }
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetFeedbackById(string id)
    {
        // Find feedback by ID using the repository
        var feedback = await _feedbackRepository.GetByIdAsync(id);

        // Return 404 if feedback is not found
        if (feedback == null)
        {
            return NotFound($"Feedback with ID {id} not found");
        }

        return Ok(feedback);
    }


    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> CreateFeedback([FromBody] Feedback feedback)
    {
        // Validate the feedback model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appointment = await _appointmentService.GetAppointmentByIdAsync(feedback.AppointmentId);

        if (appointment == null)
        {
            Console.WriteLine($"Appointment not found for Id: {feedback.AppointmentId}");
            return BadRequest("Invalid AppointmentId.");
        }
        if (appointment.PatientId != feedback.PatientId)
        {
            Console.WriteLine($"Mismatch PatientId: Appointment PatientId: {appointment.PatientId}, Feedback PatientId: {feedback.PatientId}");
            return BadRequest("Invalid PatientId.");
        }


        // Save the feedback to the database using the repository
        await _feedbackRepository.CreateAsync(feedback);

        // Return a Created response with the new feedback's details
        return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFeedback(string id, [FromBody] Feedback feedback)
    {
        // Validate the feedback model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Reuse the existing `GetFeedbackById` method to fetch feedback
        var getFeedbackResult = await GetFeedbackById(id);
        if (getFeedbackResult is NotFoundObjectResult)
        {
            return getFeedbackResult;
        }

        // Update the feedback entry
        feedback.Id = id;
        await _feedbackRepository.UpdateAsync(id, feedback);

        // Return 204 No Content on success
        return NoContent();
    }
}
