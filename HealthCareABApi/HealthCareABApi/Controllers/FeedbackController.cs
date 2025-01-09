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

    public async Task<IActionResult> GetAllFeedback()
    {
        // Call the repository method to retrieve all feedback
        // Return the result with an appropriate HTTP status code
    }

    public async Task<IActionResult> GetFeedbackById()
    {
        // Call the repository method to find feedback by ID
        // Handle the case where the feedback doesn't exist
        // Return the result
    }

    public async Task<IActionResult> CreateFeedback()
    {
        // Validate the feedback model
        // Call the repository method to insert feedback
        // Return a Created response
    }

    public async Task<IActionResult> UpdateFeedback(Feedback feedback)
    {
        // Validate the feedback model
        // Call the repository method to update feedback
        // Handle the case where feedback doesn't exist
        // Return the appropriate response
    }


}