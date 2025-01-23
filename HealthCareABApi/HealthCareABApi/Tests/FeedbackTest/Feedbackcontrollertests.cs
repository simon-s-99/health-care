using HealthCareABApi.Models;
using HealthCareABApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq; // library to simulate (mock) the behavior of objects like the repository
using Xunit;
using HealthCareABApi.Repositories.Interfaces;


namespace HealthCareABApi.Tests.FeedbackTests
{

    // Controller Tests: Ensures API endpoints return the correct responses to the client
    // Repository Tests: Ensure that the database logic is correct
    public class FeedbackControllerTests
    {
        private readonly Mock<IFeedbackRepository> _mockRepo; // Simulates the repository
        private readonly Mock<IAppointmentService> _mockAppointmentService;
        private readonly FeedbackController _controller; // The controller being tested

        public FeedbackControllerTests()
        {
            _mockRepo = new Mock<IFeedbackRepository>();
            _mockAppointmentService = new Mock<IAppointmentService>();

            // Pass both mocks to the controller
            _controller = new FeedbackController(_mockRepo.Object, _mockAppointmentService.Object);
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
            _mockRepo.Setup(repo => repo.GetPaginatedFeedbackAsync(It.IsAny<int>(), It.IsAny<int>()))
                     .ReturnsAsync(feedbackList.ToArray());

            // Act
            var result = await _controller.GetFeedback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<Feedback>>(okResult.Value); // Expect List<Feedback>
            Assert.Equal(2, returnedList.Count);
        }

        [Fact]
        public async Task GetAllFeedback_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetPaginatedFeedbackAsync(It.IsAny<int>(), It.IsAny<int>()))
                     .ReturnsAsync(Array.Empty<Feedback>());

            // Act
            var result = await _controller.GetFeedback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<Feedback>>(okResult.Value); // Expect List<Feedback>
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
            // Arrange: Set up a valid feedback object and a matching appointment
            var feedback = new Feedback
            {
                Id = "1",
                Comment = "Great service!",
                AppointmentId = "123",
                PatientId = "456",
                Rating = 5
            };

            var validAppointment = new Appointment
            {
                Id = "123",
                PatientId = "456",
                CaregiverId = "789",
                DateTime = DateTime.UtcNow,
                Status = AppointmentStatus.Completed
            };

            _mockAppointmentService
                .Setup(service => service.GetAppointmentByIdAsync(feedback.AppointmentId))
                .ReturnsAsync(validAppointment); // Simulate a valid appointment

            _mockRepo
                .Setup(repo => repo.CreateAsync(feedback))
                .Returns(Task.CompletedTask); // Simulate feedback creation

            // Act: Call the controller's CreateFeedback method
            var result = await _controller.CreateFeedback(feedback);

            // Assert: Ensure the result is a CreatedAtActionResult with correct feedback
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedFeedback = Assert.IsType<Feedback>(createdResult.Value);
            Assert.Equal(feedback.Comment, returnedFeedback.Comment);
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
