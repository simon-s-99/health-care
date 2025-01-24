using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HealthCareABApi.Tests
{
    public class AppointmentControllerTests
    {
        [Fact]
        public async Task CreateAppointment_ReturnsCreatedResult_WhenDTOIsValid()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            CreateAppointmentDTO dto = new CreateAppointmentDTO
            {
                PatientId = "678523516caf0d38580eb536",
                CaregiverId = "678523516caf0d38580eb537",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            mockService.Setup(svc => svc.CreateAppointmentAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.CreateAppointment(dto);

            // Assert
            var createdResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            mockService.Verify(svc => svc.CreateAppointmentAsync(dto), Times.Once);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsBadRequestResult_WhenDTOIsIncomplete()
        {
            // Arrange
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentController(mockService.Object);

            // CaregiverID missing
            CreateAppointmentDTO dto = new CreateAppointmentDTO
            {
                PatientId = "678523516caf0d38580eb536",
                DateTime = DateTime.Now.AddDays(14),
                Status = AppointmentStatus.Scheduled
            };

            // Mock is not reqired as the test will never hit it

            // Act
            var result = await controller.CreateAppointment(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}