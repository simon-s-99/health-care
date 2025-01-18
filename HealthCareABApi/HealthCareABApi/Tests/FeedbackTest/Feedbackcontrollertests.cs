using HealthCareABApi.Models; 
using HealthCareABApi.Repositories; 
using Microsoft.AspNetCore.Mvc; 
using Moq; // library to simulate (mock) the behavior of objects like the repository
using Xunit; 


namespace HealthCareABApi.Tests.FeedbackTests
{

    // Controller Tests: Ensures API endpoints return the correct responses to the client
    // Repository Tests: Ensure that the database logic is correct
    public class FeedbackControllerTests
    {
        private readonly Mock<IFeedbackRepository> _mockRepo; // Simulates the repository
        private readonly FeedbackController _controller; // The controller being tested

        public FeedbackControllerTests()
        {
            // Creates a mock object for the feedback repository
            _mockRepo = new Mock<IFeedbackRepository>();

            // Passes the mock repository to the controller
            _controller = new FeedbackController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllFeedback_ReturnsOkResult_WithListOfFeedback()
        {
            // Arrange
            var feedbackList = new List<Feedback>
            {
                new Feedback { Id = "1", Comment = "Great service!" },
                new Feedback { Id = "2", Comment = "Good experience." }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(feedbackList);

            // Act
            var result = await _controller.GetFeedback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<Feedback>>(okResult.Value); // Check for List<Feedback>
            Assert.Equal(2, returnedList.Count); // Verify the count
        }



        [Fact]
        public async Task GetAllFeedback_ReturnsOkResult_WithEmptyList()
        {
  
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Feedback>());

            var result = await _controller.GetFeedback();

            // Assert: Checks if the response is "Ok" and contains an empty list
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<Feedback>>(okResult.Value);
            Assert.Empty(returnedList);
        }


        [Fact]
        public async Task GetFeedbackById_ReturnsOkResult_WithFeedback()
        {
            // Arrange: Sets up the repository to return a specific feedback item
            var feedback = new Feedback { Id = "1", Comment = "Great service!" };
            _mockRepo.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(feedback);

            // Act: Calls the controller's GetFeedbackById method
            var result = await _controller.GetFeedbackById("1");

            // Assert: Checks if the response is "Ok" and contains the correct feedback
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFeedback = Assert.IsType<Feedback>(okResult.Value);
            Assert.Equal("Great service!", returnedFeedback.Comment); // Confirms the feedback comment matches
        }


        [Fact]
        public async Task GetFeedbackById_ReturnsNotFound_WithInvalidId()
        {
            _mockRepo.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Feedback)null);

            // Act: Calls the controller's GetFeedbackById method
            var result = await _controller.GetFeedbackById("1");

            // Assert: Checks if the response is "NotFound"
            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public async Task CreateFeedback_ReturnsCreatedAtAction_WithValidFeedback()
        {
            // Arrange: Sets up a valid feedback object
            var feedback = new Feedback { Id = "1", Comment = "Great service!" };

            // Act: Calls the controller's CreateFeedback method
            var result = await _controller.CreateFeedback(feedback);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedFeedback = Assert.IsType<Feedback>(createdResult.Value);
            Assert.Equal("Great service!", returnedFeedback.Comment); 
        }


        [Fact]
        public async Task CreateFeedback_ReturnsBadRequest_WithInvalidModel()
        {

            _controller.ModelState.AddModelError("Comment", "Required");

            var feedback = new Feedback { Id = "1" }; // Missing the Comment field

            var result = await _controller.CreateFeedback(feedback);

            // Assert: Checks if the response is "BadRequest"
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
