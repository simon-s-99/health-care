using HealthCareABApi.Controllers;
using HealthCareABApi.DTO;
using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HealthCareABApi.Tests
{
    public class AvailabilityControllerTests
    {
        [Fact]
        public async Task CreateAvailability_ReturnsOkResult_WhenDTOIsValid()
        {
            // Arrange
            var mockService = new Mock<IAvailabilityService>();
            var controller = new AvailabilityController(mockService.Object);

            CreateAvailabilityDTO dto = new CreateAvailabilityDTO("678523516caf0d38580eb537", DateTime.Now.AddDays(15));
            mockService.Setup(svc => svc.CreateAvailabilityAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.CreateAvailability(dto);

            // Assert
            var createdResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            mockService.Verify(svc => svc.CreateAvailabilityAsync(dto), Times.Once);
        }

        [Fact]
        public async Task CreateAvailability_ReturnsBadRequestResult_WhenDateIsInvalid()
        {
            // Arrange
            var mockService = new Mock<IAvailabilityService>();
            var controller = new AvailabilityController(mockService.Object);

            // Date is in the past
            CreateAvailabilityDTO dto = new CreateAvailabilityDTO("678523516caf0d38580eb537", new DateTime(2024, 01, 01));
            mockService.Setup(svc => svc.CreateAvailabilityAsync(dto)).ThrowsAsync(new ArithmeticException("Invalid date."));

            // Act
            var result = await controller.CreateAvailability(dto);

            // Assert
            var createdResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, createdResult.StatusCode);
            mockService.Verify(svc => svc.CreateAvailabilityAsync(dto), Times.Once);
        }
    }
}