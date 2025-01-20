using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepository;

    // Constructor to inject the feedback repository
    public FeedbackController(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetFeedback([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        // Fetch all feedback
        var feedbackList = await _feedbackRepository.GetAllAsync();

        // Sort the feedback by CreationDate in descending order (newest first)
        var sortedList = feedbackList.OrderByDescending(f => f.CreationDate).ToList();

        // Paginate the sorted list
        var paginatedList = sortedList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(paginatedList);
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

        // Find the existing feedback entry by ID
        var existingFeedback = await _feedbackRepository.GetByIdAsync(id);

        // Return 404 if feedback is not found
        if (existingFeedback == null)
        {
            return NotFound($"Feedback with ID {id} not found");
        }

        // Update the feedback entry
        feedback.Id = id;
        await _feedbackRepository.UpdateAsync(id, feedback);

        // Return 204 No Content on success
        return NoContent();
    }
}
