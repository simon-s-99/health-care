using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepository;

    //constructor
    public FeedbackController(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFeedback()
    {
        // Call the repository and tell it to retrieve all feedback
        var feedbackList = await _feedbackRepository.GetAllAsync();

        return Ok(feedbackList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFeedbackById(string id)
    {
        // Call the repository method to find feedback by ID
        var feedback = await _feedbackRepository.GetByIdAsync(id);

        if (feedback == null)
        {
            return NotFound($"Feedback with ID {id} not found");
        }

        return Ok(feedback);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeedback([FromBody] Feedback feedback)
    {
        // Validate the feedback model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Call the repository method to insert feedback
        await _feedbackRepository.CreateAsync(feedback);

        // Return a Created response
        return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
    }

    [HttpGet("TestConnection")]
    public IActionResult TestConnection()
    {
        return Ok("Hello from the backend!");
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFeedback(string id, [FromBody] Feedback feedback)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Call the repository method to update feedback
        var existingFeedback = await _feedbackRepository.GetByIdAsync(id);
        if (existingFeedback == null)
        {
            return NotFound($"Feedback with ID {id} not found");
        }

        feedback.Id = id;
        await _feedbackRepository.UpdateAsync(id, feedback);

        //204 response
        return NoContent();
    }


}